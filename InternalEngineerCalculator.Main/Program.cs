using System.Net.Mime;

namespace InternalEngineerCalculator.Main;

static class Program
{
	static void Main()
	{
		Console.WriteLine("Hello, world!");
	}
}

public enum TokenType
{
	Number,

	Plus,
	Minus,
	Divide,
	Multiply,
	LeftParenthesis,
	RightParenthesis,
	Dot
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

	public float Value { get; set; }

	public override string ToString() => $"{nameof(NumberToken)} : {Value}";
}

internal sealed class OperationToken (string text, TokenType type) : Token
{

	public override string Text { get; set; } = text;

	public override TokenType Type { get; set; } = type;

	public override string ToString() => Type.ToString();
}