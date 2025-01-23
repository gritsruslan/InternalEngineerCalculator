using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

public sealed class VariableAssignmentExpression(Token identifierToken, Expression expression) : AssignmentExpression
{
	private Token IdentifierToken { get; } = identifierToken;

	public override Expression Expression { get; } = expression;

	public override string Name => IdentifierToken.ValueString;
}