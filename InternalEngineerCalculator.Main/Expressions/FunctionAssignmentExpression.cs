using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal class FunctionAssignmentExpression(string name, ImmutableArray<string> args, Expression functionExpression) : AssignmentExpression
{
	private ImmutableArray<string> _args = args;

	public override string Name { get; } = name;

	public IReadOnlyList<string> Args => _args;

	public Expression FunctionExpression { get; } = functionExpression;

	public override TokenType Type => TokenType.FunctionAssignmentExpression;
}