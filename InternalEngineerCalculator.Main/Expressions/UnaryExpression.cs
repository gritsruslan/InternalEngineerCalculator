using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class UnaryExpression(NonValueToken unaryOperation, Expression expression, UnaryExpressionType type) : Expression
{
	public Expression Expression { get; } = expression;

	public NonValueToken UnaryOperation { get; } = unaryOperation;

	public UnaryExpressionType Type { get; } = type;
}