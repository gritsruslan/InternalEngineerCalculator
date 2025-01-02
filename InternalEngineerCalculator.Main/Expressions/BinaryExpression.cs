using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class BinaryExpression(Expression left, NonValueToken operation, Expression right) : Expression
{
	public Expression Left => left;

	public NonValueToken Operation => operation;

	public Expression Right => right;
}