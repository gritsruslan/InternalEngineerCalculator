using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class UnaryExpression(Expression expression, UnaryExpressionType type) : Expression
{
	public Expression Expression { get; } = expression;

	public UnaryExpressionType Type { get; } = type;
}