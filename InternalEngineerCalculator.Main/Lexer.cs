using System.Globalization;
using System.Text;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Exceptions;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main;

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

	private readonly HashSet<char> _singleChars = ['+', '-', '*', '/', '(', ')', '^', ',', '='];

	private readonly HashSet<char> _separatorChars = [' ', '\t', '\r', '\0'];

	private bool IsSingleChar(char chr) => _singleChars.Contains(chr);

	private bool IsSeparator(char chr) => _singleChars.Contains(chr) || _separatorChars.Contains(chr);

	public ICollection<Token> Tokenize()
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

		return identifierToken;
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
			throw new CalculatorException($"The entry \"{tokenString}\" cannot be represented as a number.");

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
			',' => TokenType.Comma,
			'=' => TokenType.EqualSign,
			_ => throw new CalculatorException("Unknown single char operator!")
		};

		var position = _position;
		var valueString = Current.ToString();

		Next();

		return new NonValueToken(singleCharTokenType, position, valueString);
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