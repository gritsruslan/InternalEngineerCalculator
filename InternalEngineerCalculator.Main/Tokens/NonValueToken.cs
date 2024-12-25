namespace InternalEngineerCalculator.Main.Tokens;

internal sealed class NonValueToken (TokenType type, int position, string valueString)
	: Token(type, position, valueString)
{
	public override string ToString() => Type.ToString();
}