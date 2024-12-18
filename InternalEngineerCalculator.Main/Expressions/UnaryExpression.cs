using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class UnaryExpression(NonValueToken unaryOperation, Expression expression) : Expression
{
	public Expression Expression { get; set; } = expression;

	public NonValueToken UnaryOperation { get; set; } = unaryOperation;

	public override TokenType Type => TokenType.UnaryOperation;
}