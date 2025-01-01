using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class UnaryExpression(NonValueToken unaryOperation, Expression expression) : Expression
{
	public Expression Expression => expression;

	public NonValueToken UnaryOperation => unaryOperation;
}