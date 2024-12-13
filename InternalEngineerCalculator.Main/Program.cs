using System.Globalization;
using System.Text;
using InternalEngineerCalculator.Main.Common;

namespace InternalEngineerCalculator.Main;

static class Program
{
	private static void Main()
	{
		var Iec = new InternalEngineerCalculator();
#if DEBUG
		Iec.Start(true)
#else
		Iec.Start(false);
#endif
	}
}

internal class InternalEngineerCalculator
{
	public readonly ConsoleColor DefaultColor = ConsoleColor.Gray;

	private bool _showExpressionTree = false;

	public void Start(bool isDebugMode)
	{
		if(!isDebugMode)
			Console.WriteLine($"InternalEngineerCalculator by @gritsruslan!");
		else
			Console.WriteLine($"InternalEngineerCalculator by @gritsruslan! : Debug Mode");

		while (true)
		{
			try
			{
				Console.Write("> ");
				var input = Console.ReadLine();

				if (string.IsNullOrWhiteSpace(input))
					continue;

				if (input[0] == '#')
				{
					ProcessCommandLine(input);
					continue;
				}

				var lexemes = new Lexer(input).Tokenize();
				var parser = new Parser(lexemes);
				var expression = parser.ParseExpression();
				var evaluator = new Evaluator();
				var result = evaluator.Evaluate(expression);

				if(_showExpressionTree)
					expression.PrettyPrint();

				Console.WriteLine($"Result : {result}");
			}
			catch (CalculatorException exception)
			{
				Console.ForegroundColor = ConsoleColor.DarkMagenta;
				Console.WriteLine(exception.Message);
				Console.WriteLine();
				Console.ForegroundColor = DefaultColor;
			}
			catch (Exception exception)
			{
				Console.ForegroundColor = ConsoleColor.DarkRed;

				if(!isDebugMode)
					Console.WriteLine("An unhandled error occurred while the program was running. Please contact us!");
				else
					Console.WriteLine(exception);

				Console.WriteLine();
				Console.ForegroundColor = DefaultColor;
			}
		}
	}

	private void ProcessCommandLine(string commandLine)
	{
		var commandLineNames =
			commandLine.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(str => str.ToLower()).ToArray();
		var command = commandLineNames[0];

		if(command == "#exit")
			Environment.Exit(0);
		else if(command == "#clear")
			Console.Clear();
		else if (command == "#help")
		{
			string helpString =
				"""

				InternalEngineerCalculator by @gritsruslan!

				Available math operators : + - * / ^
				Examples:
				12 + 3 * (2 - 1)
				2 ^ 3 + 52
				1/20 + 1

				Available commands :
				#exit - exit calculator
				#clear - clear console output
				#help - output short calculator guide
				#showexpressiontree - enable showing expression trees
				#unshowexpressiontree - disable showing expression trees

				""";
			Console.WriteLine(helpString);
		}
		else if (command == "#showexpressiontree")
			_showExpressionTree = true;
		else if (command == "#unshowexpressiontree")
			_showExpressionTree = false;
		else
		{
			Console.WriteLine($"Unknown command : {command}");
		}
	}
}

internal class CalculatorException(string message) : Exception(message);

internal sealed class CalculatorDivideByZeroException() : CalculatorException("Divide by zero is not allowed!");
internal sealed class UnexpectedTokenException : CalculatorException
{
	public UnexpectedTokenException(string message) : base(message){}

	public UnexpectedTokenException(string received, string expected) : base(
		$"Unexpected operator {received}, expected : {expected}") {}

	public UnexpectedTokenException(TokenType received, TokenType expected) : base(
		$"Unexpected operator {received}, expected : {expected}") {}
}

internal sealed class EndOfInputException() : Exception("Unexpected end of input!");

public enum TokenType
{
	Number,

	Plus,
	Minus,
	Divide,
	Multiply,
	Pow,
	OpenParenthesis,
	CloseParenthesis,
	EndOfLine,
	Identifier,

	BinaryExpression,
	UnaryOperation
}

internal abstract class Token(TokenType type, int position, string valueString)
{
	public string ValueString => valueString;

	public int Position => position;

	public TokenType Type => type;

	public int Length => ValueString.Length;

	public override string ToString() => Type.ToString();
}

internal sealed class NumberToken(string valueString, int position, double value)
	: Token(TokenType.Number, position, valueString)
{
	public double Value => value;

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

internal sealed class UnaryExpression(NonValueToken unaryOperation, Expression expression) : Expression
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

	public override TokenType Type => TokenType.BinaryExpression;
}

internal static class ExpressionExtensions
{
	public static void PrettyPrint(this Expression expression, string offset = "")
	{
		if(expression is NumberExpression numberExpression)
			Console.WriteLine(offset + $"Number : {numberExpression.Token.ValueString}");

		if (expression is UnaryExpression unaryExpression)
		{
			offset += "  ";
			Console.WriteLine(offset + $"Operation {unaryExpression.UnaryOperation.ValueString}");
			unaryExpression.Expression.PrettyPrint(offset);
		}

		if (expression is BinaryExpression binaryExpression)
		{
			Console.WriteLine(offset + "Binary Expression");
			offset += "  ";
			binaryExpression.Left.PrettyPrint(offset);
			Console.WriteLine(offset + $"Operation {binaryExpression.Operation.ValueString}");
			binaryExpression.Right.PrettyPrint(offset);
		}
	}
}

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

	public Expression ParseExpression(int parentPrecedence = 0)
	{
		Expression left;
		var unaryOperatorPrecedence = GetUnaryOperatorPrecedence(Current!);
		if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
		{
			var operatorToken = Current as NonValueToken;
			Next();
			var operand = ParseExpression(unaryOperatorPrecedence);
			left = new UnaryExpression(operatorToken!, operand);
		}
		else
		{
			left = ParseParenthesis();
		}

		while (true)
		{
			if(Current is null)
				break;

			if (Current is not NonValueToken operatorToken)
				throw new UnexpectedTokenException(Current.Type.ToString(), "math operator");

			var precedence = GetOperationPrecedence(operatorToken);

			if (precedence == 0 || precedence <= parentPrecedence)
				break;

			Next();
			var right = ParseExpression(precedence);
			left = new BinaryExpression(left, operatorToken, right);
		}

		return left;
	}

	private static int GetUnaryOperatorPrecedence(Token token)
	{
		return token.Type switch
		{
			TokenType.Plus => 3,
			TokenType.Minus => 3,
			_ => 0
		};
	}
	private static int GetOperationPrecedence(NonValueToken token)
	{
		return token.Type switch
		{
			TokenType.Plus => 1,
			TokenType.Minus => 1,

			TokenType.Multiply => 2,
			TokenType.Divide => 2,

			TokenType.Pow => 3,

			_ => 0
		};
	}

	private Expression ParseParenthesis()
	{
		if (Current == null)
			throw new EndOfInputException();

		Expression expr;
		if (Current.Type == TokenType.OpenParenthesis)
		{
			Next();
			expr = ParseExpression();

			if (Current == null)
				throw new EndOfInputException();

			if (Current.Type != TokenType.CloseParenthesis)
				throw new UnexpectedTokenException(Current.Type,TokenType.CloseParenthesis );

			Next();
			return expr;
		}

		if (Current is not NumberToken numberToken)
			throw new UnexpectedTokenException(Current.Type,TokenType.CloseParenthesis);

		Next();
		expr = new NumberExpression(numberToken);
		return expr;
	}
}

internal sealed class Evaluator
{
	public double Evaluate(Expression expression)
	{
		if (expression is NumberExpression ne)
			return ne.Token.Value;

		if (expression is UnaryExpression ue)
		{
			var expr = Evaluate(ue.Expression);

			if (ue.UnaryOperation.Type == TokenType.Minus)
				expr *= -1;
			// Other unary operators
			return expr;
		}

		var binExpression = expression as BinaryExpression;

		var left = Evaluate(binExpression!.Left);
		var operation = binExpression.Operation;
		var right = Evaluate(binExpression!.Right);

		double result = operation.Type switch
		{
			TokenType.Plus => left + right,
			TokenType.Minus => left - right,
			TokenType.Multiply => left * right,
			TokenType.Divide => right == 0 ? throw new CalculatorDivideByZeroException() : left/right,
			TokenType.Pow => Math.Pow(left, right),
			_ => throw new CalculatorException("Unknown math operator!")
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

	private readonly HashSet<char> _singleChars = ['+', '-', '*', '/', '(', ')', '^'];

	private readonly HashSet<char> _separatorChars = [' ', '\t', '\r', '\0'];

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

		var identifierToken = ProcessIdentifier();

		throw new CalculatorException($"Unknown token {identifierToken}");
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
				throw new CalculatorException("Invalid number token!");

			tokenString += Current;

			Next();
		}
		while (char.IsDigit(Current) || Current == dot);

		if (!double.TryParse(tokenString, NumberStyles.Float, CultureInfo.InvariantCulture, out var tokenValue))
			throw new Exception($"The entry \"{tokenString}\" cannot be represented as a number.");

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
			'^' => TokenType.Pow,
			_ => throw new CalculatorException("Unknown single char operator!")
		};

		Next();

		return new NonValueToken(singleCharTokenType, _position, Current.ToString());
	}

	private NonValueToken ProcessIdentifier()
	{
		StringBuilder sb = new StringBuilder();
		int startPosition = _position;
		while (!IsSeparator(Current))
		{
			sb.Append(Current);
			Next();
		}

		return new NonValueToken(TokenType.Identifier, startPosition, sb.ToString());
	}
}