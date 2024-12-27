using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal class VariableAssignmentExpression(Token identifierToken, Expression variableValueExpression) : AssignmentExpression
{
	public Token IdentifierToken { get; } = identifierToken;

	public Expression VariableValueExpression { get; } = variableValueExpression;

	public override string Name => IdentifierToken.ValueString;

	public override TokenType Type => TokenType.VariableAssignmentExpression;
}