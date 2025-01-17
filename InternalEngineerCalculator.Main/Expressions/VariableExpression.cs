using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class VariableExpression(Token identifierToken) : Expression
{
	private Token Token { get; } = identifierToken;
	
	public string Name => Token.ValueString;
}