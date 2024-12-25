namespace InternalEngineerCalculator.Main.Tokens;

internal abstract class Token(TokenType type, int position, string valueString)
{
	public string ValueString => valueString;

	public int Position => position;

	public TokenType Type => type;

	public int Length => ValueString.Length;

	public override string ToString() => Type.ToString();
}