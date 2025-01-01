using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal class VariableAssignmentExpression(Token identifierToken, Expression expression) : AssignmentExpression
{
	private Token IdentifierToken => identifierToken;

	public override Expression Expression => expression;

	public override string Name => IdentifierToken.ValueString;
}