using System.Globalization;
using System.Text;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main;

/// <summary> Converts the input string into a collection of meaningful tokens </summary>
public sealed class Lexer
{
	private string _code = string.Empty;

	private int _position;

	private readonly IReadOnlyDictionary<char, TokenType> _singleCharTokens = new Dictionary<char, TokenType>
	{
		{'+', TokenType.Plus},
		{'-', TokenType.Minus},
		{'*', TokenType.Multiply},
		{'/', TokenType.Divide},
		{'(', TokenType.OpenParenthesis},
		{')', TokenType.CloseParenthesis},
		{'^', TokenType.Pow},
		{',', TokenType.Comma},
		{'=', TokenType.EqualSign},
		{'!', TokenType.Factorial},
		{'|', TokenType.ModulePipe},
		{'%', TokenType.Remainder}
	};

	private readonly HashSet<char> _separatorChars = [' ', '\t', '\r', '\0'];

	private char Current => _position < _code.Length ? _code[_position] : '\0';

	private void Next() => _position++;

	private bool IsSingleChar(char chr) => _singleCharTokens.ContainsKey(chr);

	// single char tokens are also separators
	private bool IsSeparator(char chr) => IsSingleChar(chr) || _separatorChars.Contains(chr);

	private void SkipWhitespaces()
	{
		while(Current != '\0' && char.IsWhiteSpace(Current))
			Next();
	}

	public Result<ICollection<Token>> Tokenize(string code)
	{
		List<Token> tokens = [];
		_code = code;

		while (true)
		{
			var tokenResult = NextToken();
			if (!tokenResult.TryGetValue(out var token))
				return tokenResult.Error;

			if(token.Type == TokenType.EndOfLine)
				break;

			tokens.Add(token);
		}

		_code = string.Empty;
		_position = 0;

		return tokens;
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
				return new Error("Invalid number token in expression!");

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

		if (!_singleCharTokens.TryGetValue(Current, out var singleCharTokenType))
			throw new Exception("Unknown single char operator!");

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