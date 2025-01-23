namespace InternalEngineerCalculator.Main.Tokens;

internal sealed class NumberToken(string valueString, double value)
	: Token(TokenType.Number, valueString)
{
	public double Value => value;

	public override string ToString() => $"{nameof(NumberToken)} : {Value}";
}