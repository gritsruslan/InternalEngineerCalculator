using System.Globalization;
using InternalEngineerCalculator.Main.Common;

namespace InternalEngineerCalculator.Main;

static class Program
{
	static void Main()
	{
		while (true)
		{
			Console.Write("> ");
			var code = Console.ReadLine();
			if (code is null)
				continue;

			var tokens = new Lexer(code).Tokenize();

			foreach (var token in tokens)
				Console.WriteLine(token.ToString());

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
	Unknown
}

internal abstract class Token
{
	public abstract string Text { get; set; }

	public abstract TokenType Type { get; set; }

	public override string ToString() => nameof(TokenType);
}

internal sealed class NumberToken(string text, float value) : Token
{
	public override string Text { get; set; } = text;

	public override TokenType Type { get; set; } = TokenType.Number;

	public float Value { get; set; } = value;

	public override string ToString() => $"{nameof(NumberToken)} : {Value}";
}

internal sealed class NonValueToken (string text, TokenType type) : Token
{

	public override string Text { get; set; } = text;

	public override TokenType Type { get; set; } = type;

	public override string ToString() => Type.ToString();
}

internal sealed class Lexer(string code)
{
	private List<Token> _tokens = [];

	private string _code = code;

	private int _index = 0;

	private char Current => _index < _code.Length ? _code[_index] : '\0';

	private void Next() => _index++;

	private void SkipWhitespaces()
	{
		while(Current != '\0' && char.IsWhiteSpace(Current))
			Next();
	}

	private readonly HashSet<char> _singleChars = ['+', '-', '*', '/', '(', ')'];

	private bool IsSingleChar(char chr) => _singleChars.Contains(chr);

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
			return new NonValueToken("\0", TokenType.EndOfLine);

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

		return new NumberToken(tokenString, tokenValue);
	}

	private Option<NonValueToken> ProcessIfSingleCharToken()
	{
		if (!IsSingleChar(Current))
			return Option<NonValueToken>.None;

		NonValueToken singleCharToken = Current switch
		{
			'+' => new NonValueToken("+", TokenType.Plus),
			'-' => new NonValueToken("-", TokenType.Minus),
			'*' => new NonValueToken("-", TokenType.Multiply),
			'/' => new NonValueToken("-", TokenType.Divide),
			'(' => new NonValueToken("(", TokenType.OpenParenthesis),
			')' => new NonValueToken(")", TokenType.CloseParenthesis),
			_ => throw new Exception()
		};

		Next();

		return singleCharToken;
	}
}