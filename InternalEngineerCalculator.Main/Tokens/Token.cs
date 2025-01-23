namespace InternalEngineerCalculator.Main.Tokens;

public abstract class Token(TokenType type, string valueString)
{
	public string ValueString { get; } = valueString;

	public TokenType Type { get; } = type;

	public override string ToString() => Type.ToString();
}