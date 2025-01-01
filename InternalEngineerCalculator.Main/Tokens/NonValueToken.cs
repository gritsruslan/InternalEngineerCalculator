namespace InternalEngineerCalculator.Main.Tokens;

internal sealed class NonValueToken (TokenType type, string valueString)
	: Token(type,valueString)
{
	public override string ToString() => Type.ToString();
}