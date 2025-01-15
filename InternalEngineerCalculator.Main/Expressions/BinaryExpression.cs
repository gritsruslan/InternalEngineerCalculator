using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class BinaryExpression(Expression left, BinaryOperationType operationType, Expression right) : Expression
{
	public Expression Left { get; } = left;

	public Expression Right { get; } = right;


	public BinaryOperationType OperationType { get; } = operationType;
}