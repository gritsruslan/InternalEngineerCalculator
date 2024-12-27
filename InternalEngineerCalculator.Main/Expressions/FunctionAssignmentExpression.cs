using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal class FunctionAssignmentExpression(string name, List<string> args, Expression functionExpression) : AssignmentExpression
{
	private List<string> _args = args;

	public override string Name { get; } = name;

	public IReadOnlyList<string> Args => _args;

	public Expression FunctionExpression { get; } = functionExpression;

	public override TokenType Type => TokenType.FunctionAssignmentExpression;
}