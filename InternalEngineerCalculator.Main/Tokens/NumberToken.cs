namespace InternalEngineerCalculator.Main;

internal sealed class NumberToken(string valueString, int position, double value)
	: Token(TokenType.Number, position, valueString)
{
	public double Value => value;

	public override string ToString() => $"{nameof(NumberToken)} : {Value}";
}