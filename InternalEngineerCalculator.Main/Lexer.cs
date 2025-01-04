using System.Globalization;
using System.Text;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main;

internal sealed class Lexer(string code)
{
	private readonly List<Token> _tokens = [];

	private readonly string _code = code;

	private int _position;

	private readonly HashSet<char> _singleChars = ['+', '-', '*', '/', '(', ')', '^', ',', '=', '!', '|'];

	private readonly HashSet<char> _separatorChars = [' ', '\t', '\r', '\0'];

	private char Current => _position < _code.Length ? _code[_position] : '\0';

	private void Next() => _position++;

	private bool IsSingleChar(char chr) => _singleChars.Contains(chr);

	private bool IsSeparator(char chr) => _singleChars.Contains(chr) || _separatorChars.Contains(chr);

	private void SkipWhitespaces()
	{
		while(Current != '\0' && char.IsWhiteSpace(Current))
			Next();
	}

	public Result<ICollection<Token>> Tokenize()
	{
		while (true)
		{
			var tokenResult = NextToken();
			if (!tokenResult.TryGetValue(out var token))
				return tokenResult.Error;

			if(token.Type == TokenType.EndOfLine)
				break;

			_tokens.Add(token);
		}

		return _tokens;
	}

	private Result<Token> NextToken()
	{
		SkipWhitespaces();

		if (Current == '\0')
			return new NonValueToken(TokenType.EndOfLine, "\0");

		var numberTokenOptResult = ProcessIfNumberToken();
		if (!numberTokenOptResult.TryGetValue(out var numberTokenOpt))
			return numberTokenOptResult.Error;

		if (numberTokenOpt.IsSome)
			return numberTokenOpt.Unwrap();

		var singleCharToken = ProcessIfSingleCharToken();
		if (singleCharToken.IsSome)
			return singleCharToken.Unwrap();

		var identifierToken = ProcessIdentifier();

		return identifierToken;
	}

	private Result<Option<NumberToken>> ProcessIfNumberToken()
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
				return new Error("Invalid number token!");

			tokenString += Current;

			Next();
		}
		while (char.IsDigit(Current) || Current == dot);

		if (!double.TryParse(tokenString, NumberStyles.Float, CultureInfo.InvariantCulture, out var tokenValue))
			return new Error($"The entry \"{tokenString}\" cannot be represented as a number.");

		return Option<NumberToken>.Some(new NumberToken(tokenString, tokenValue));
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
			',' => TokenType.Comma,
			'=' => TokenType.EqualSign,
			'!' => TokenType.Factorial,
			'|' => TokenType.Pipe,
			_ => throw new Exception("Unknown single char operator!")
		};

		var valueString = Current.ToString();

		Next();

		return new NonValueToken(singleCharTokenType, valueString);
	}

	private NonValueToken ProcessIdentifier()
	{
		StringBuilder sb = new StringBuilder();

		while (!IsSeparator(Current))
		{
			sb.Append(Current);
			Next();
		}

		return new NonValueToken(TokenType.Identifier, sb.ToString());
	}
}