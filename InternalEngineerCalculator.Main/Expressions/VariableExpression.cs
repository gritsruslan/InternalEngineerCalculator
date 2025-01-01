using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal class VariableExpression(Token identifierToken) : Expression
{
	private readonly Token _variableToken = identifierToken;

	public string Name => _variableToken.ValueString;
}