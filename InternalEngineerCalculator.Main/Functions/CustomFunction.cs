using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Expressions;

namespace InternalEngineerCalculator.Main.Functions;

internal sealed class CustomFunction(string name, ImmutableArray<FunctionArgument> arguments, Expression functionExpression)
	: Function(name)
{
	public Expression FunctionExpression { get; } = functionExpression;

	public ImmutableArray<FunctionArgument> Arguments { get; } = arguments;

	public override bool IsBaseFunction => false;

	public override int CountOfArgs => Arguments.Length;
}