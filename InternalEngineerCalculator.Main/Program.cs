using System.Globalization;
using System.Text;
using InternalEngineerCalculator.Main.Common;

namespace InternalEngineerCalculator.Main;

static class Program
{
	private static void Main()
	{
		while (true)
		{
			Console.Write("> ");
			var code = Console.ReadLine();
			if (code is null)
				continue;

			var tokens = new Lexer(code).Tokenize();

			var evaluator = new Evaluator();
			var parser = new Parser(tokens);
			var expr = parser.ParseExpressionMain();

			if (expr is NumberExpression num)
			{
				Console.WriteLine($"Number : {num.Token.Value}");
			}
			else if(expr is BinaryExpression be)
			{
				Console.WriteLine(be);
				var str = be!.ToCustomString(new StringBuilder(), "");

				Console.WriteLine(str);
				Console.WriteLine("Result : " + evaluator.Evaluate(be));
			}


		}
	}



}

public enum TokenType
{
	Number,

	Plus,
	Minus,
	Divide,
	Multiply,
	OpenParenthesis,
	CloseParenthesis,
	Dot,
	EndOfLine,
	Unknown,

	BinaryExpression,
	UnaryOperation
}

internal abstract class Token(TokenType type, int position, string valueString)
{
	public string ValueString => valueString;

	public int Position => position;

	public TokenType Type => type;

	public int Length => ValueString.Length;

	public override string ToString() => nameof(TokenType);
}

internal sealed class NumberToken(string valueString, int position, float value)
	: Token(TokenType.Number, position, valueString)
{
	public float Value => value;

	public override string ToString() => $"{nameof(NumberToken)} : {Value}";
}

internal sealed class NonValueToken (TokenType type, int position, string valueString)
	: Token(type, position, valueString)
{
	public override string ToString() => Type.ToString();
}

internal abstract class Expression
{
	public abstract TokenType Type { get; }
}

internal sealed class NumberExpression(NumberToken token) : Expression
{
	public NumberToken Token { get; } = token;

	public override TokenType Type => TokenType.Number;
}

internal sealed class UnaryExpression(Expression expression, NonValueToken unaryOperation) : Expression
{
	public Expression Expression { get; set; } = expression;

	public NonValueToken UnaryOperation { get; set; } = unaryOperation;

	public override TokenType Type => TokenType.UnaryOperation;
}
internal sealed class BinaryExpression(Expression left, NonValueToken operation, Expression right) : Expression
{
	public Expression Left { get; } = left;
	public NonValueToken Operation { get; } = operation;
	public Expression Right { get; } = right;

	public string ToCustomString(StringBuilder sb, string offset)
	{
		sb.Append(offset + "BinaryExpression\n");
		offset += "	";

		if (Left is BinaryExpression leftExpr)
			leftExpr.ToCustomString(sb, offset);
		else if (Left is NumberExpression leftNum)
			sb.Append(offset + leftNum.Token + "\n");

		sb.Append(offset + Operation + " " + Operation.ValueString + "\n");

		if (Right is BinaryExpression rightExpr)
			rightExpr.ToCustomString(sb, offset);
		else if (Right is NumberExpression rightNum)
			sb.Append(offset + rightNum.Token + "\n");

		return sb.ToString();
	}

	public override TokenType Type => TokenType.BinaryExpression;
}

//test
internal sealed class Parser
{
	private List<Token> _tokens;

	private int _position = 0;

	private Token? Current => _position < _tokens.Count ? _tokens[_position] : null;

	private void Next() => _position++;
	public Parser(List<Token> tokens)
	{
		_tokens = tokens;
	}

	public Expression ParseExpressionMain(int parentPrecedence = 0)
	{
		var left = ParseParenthesis();

		while (true)
		{
			if(Current is null)
				break;

			if (Current is not NonValueToken operatorToken)
				throw new Exception($"Unexpected operator : {Current}");

			var precedence = GetOperationPrecedence(operatorToken);

			if (precedence == 0 || precedence <= parentPrecedence)
				break;

			Next();
			var right = ParseExpressionMain(precedence);
			left = new BinaryExpression(left, operatorToken, right);
		}

		return left;
	}

	private static int GetOperationPrecedence(NonValueToken token)
	{
		return token.Type switch
		{
			TokenType.Plus => 1,
			TokenType.Minus => 1,

			TokenType.Multiply => 2,
			TokenType.Divide => 2,

			_ => 0
		};
	}

	private Expression ParseParenthesis()
	{
		if (Current == null)
			throw new Exception("Unexpected end of input!");

		Expression expr;
		if (Current.Type == TokenType.OpenParenthesis)
		{
			Next();
			expr = ParseExpressionMain();

			if (Current == null || Current.Type != TokenType.CloseParenthesis)
				throw new Exception($"Unexpected token : {Current}, expected CloseParenthesis");

			Next();
			return expr;
		}

		if (Current is not NumberToken numberToken)
			throw new Exception($"Unexpected token : {Current}, expected NumberToken");

		Next();
		expr = new NumberExpression(numberToken);
		return expr;
	}
}

internal sealed class Evaluator
{
	public float Evaluate(Expression expression)
	{
		if (expression is NumberExpression ne)
			return ne.Token.Value;

		var binExpression = expression as BinaryExpression;

		var left = Evaluate(binExpression!.Left);
		var operation = binExpression.Operation;
		var right = Evaluate(binExpression!.Right);

		float result = operation.Type switch
		{
			TokenType.Plus => left + right,
			TokenType.Minus => left - right,
			TokenType.Multiply => left * right,
			TokenType.Divide => left / right,
			_ => throw new Exception()
		};

		return result;
	}
}

internal sealed class Lexer(string code)
{
	private List<Token> _tokens = [];

	private string _code = code;

	private int _position;

	private char Current => _position < _code.Length ? _code[_position] : '\0';

	private void Next() => _position++;

	private void SkipWhitespaces()
	{
		while(Current != '\0' && char.IsWhiteSpace(Current))
			Next();
	}

	private readonly HashSet<char> _singleChars = ['+', '-', '*', '/', '(', ')'];

	private readonly HashSet<char> _separatorChars = [' ', '\t', '\r'];

	private bool IsSingleChar(char chr) => _singleChars.Contains(chr);

	private bool IsSeparator(char chr) => _singleChars.Contains(chr) || _separatorChars.Contains(chr);

	public List<Token> Tokenize()
	{
		while (true)
		{
			var token = NextToken();

			if(token.Type == TokenType.EndOfLine)
				break;

			_tokens.Add(token);
		}

		return _tokens;
	}

	private Token NextToken()
	{
		SkipWhitespaces();

		if (Current == '\0')
			return new NonValueToken(TokenType.EndOfLine, -1, "\0");

		var numberTokenOpt = ProcessIfNumberToken();
		if (numberTokenOpt.IsSome)
			return numberTokenOpt.Unwrap();

		var singleCharToken = ProcessIfSingleCharToken();
		if (singleCharToken.IsSome)
			return singleCharToken.Unwrap();

		throw new Exception("Unknown Token");
	}

	private Option<NumberToken> ProcessIfNumberToken()
	{
		if (!char.IsDigit(Current))
			return Option<NumberToken>.None;

		const char dot = '.';
		var tokenString = string.Empty;
		bool hasDot = false;
		int startPosition = _position;

		do
		{
			if (Current == dot && !hasDot)
				hasDot = true;
			else if (Current == dot && hasDot)
				throw new Exception();

			tokenString += Current;

			 Next();
		}
		while (char.IsDigit(Current) || Current == dot);

		if (!float.TryParse(tokenString, NumberStyles.Float, CultureInfo.InvariantCulture, out var tokenValue))
			throw new Exception();

		return new NumberToken(tokenString,startPosition, tokenValue);
	}

	private Option<NonValueToken> ProcessIfSingleCharToken()
	{
		if (!IsSingleChar(Current))
			return Option<NonValueToken>.None;

		TokenType singleCharTokenType = Current switch
		{
			'+' => TokenType.Plus,
			'-' => TokenType.Minus,
			'*' => TokenType.Multiply,
			'/' => TokenType.Divide,
			'(' => TokenType.OpenParenthesis,
			')' => TokenType.CloseParenthesis,
			_ => throw new Exception()
		};

		Next();

		return new NonValueToken(singleCharTokenType, _position, Current.ToString());
	}
}