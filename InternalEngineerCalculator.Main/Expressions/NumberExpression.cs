using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class NumberExpression(NumberToken token) : Expression
{
	public NumberToken Token { get; } = token;

	public double Value => Token.Value;
}