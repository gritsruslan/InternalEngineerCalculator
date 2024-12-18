namespace InternalEngineerCalculator.Main;

internal sealed class NumberExpression(NumberToken token) : Expression
{
	public NumberToken Token { get; } = token;

	public override TokenType Type => TokenType.Number;
}