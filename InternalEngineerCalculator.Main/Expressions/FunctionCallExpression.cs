using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main;

internal sealed class FunctionCallExpression(string name, Expression[] arguments) : Expression
{
	public readonly string Name = name;

	public readonly Expression[] Arguments = arguments;

	public override TokenType Type => TokenType.FunctionCall;

	public int CountOfArgs => Arguments.Length;
}