using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class BinaryExpression(Expression left, NonValueToken operation, Expression right) : Expression
{
	public Expression Left { get; } = left;

	public NonValueToken Operation { get; } = operation;

	public Expression Right { get; } = right;
}