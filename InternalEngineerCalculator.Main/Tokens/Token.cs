namespace InternalEngineerCalculator.Main.Tokens;

public abstract class Token(TokenType type, string valueString)
{
	public string ValueString => valueString;

	public TokenType Type => type;

	public override string ToString() => Type.ToString();
}