using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class FunctionAssignmentExpression(string name, ImmutableArray<string> args, Expression expression)
	: AssignmentExpression
{
	public ImmutableArray<string> Args => args;

	public override string Name => name;

	public override Expression Expression => expression;
}