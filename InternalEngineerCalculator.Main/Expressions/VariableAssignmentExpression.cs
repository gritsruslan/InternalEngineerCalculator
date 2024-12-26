using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal class VariableAssignmentExpression(Token identifierToken, Expression variableValueExpression) : Expression
{
	public Token IdentifierToken { get; } = identifierToken;

	public Expression VariableValueExpression { get; } = variableValueExpression;

	public string VariableName => identifierToken.ValueString;

	public override TokenType Type => TokenType.VariableAssignmentExpression;
}