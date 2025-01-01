namespace InternalEngineerCalculator.Main.Tokens;

internal abstract class Token(TokenType type, string valueString)
{
	public string ValueString => valueString;

	public TokenType Type => type;

	public int Length => ValueString.Length;

	public override string ToString() => Type.ToString();
}