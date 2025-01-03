using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class FunctionAssignmentExpression(string name, ImmutableArray<string> args, Expression expression)
	: AssignmentExpression
{
	public ImmutableArray<string> Args { get; } = args;

	public override string Name { get; } = name;

	public override Expression Expression { get; } = expression;
}