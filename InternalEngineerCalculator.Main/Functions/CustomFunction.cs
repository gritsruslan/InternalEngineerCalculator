using InternalEngineerCalculator.Main.Expressions;

namespace InternalEngineerCalculator.Main.Functions;

internal sealed class CustomFunction(string name, IReadOnlyList<FunctionArgument> arguments, Expression functionExpression)
	: Function(name)
{
	public Expression FunctionExpression { get; } = functionExpression;
	public IReadOnlyList<FunctionArgument> Arguments { get; } = arguments;
	public override bool IsBaseFunction => false;
	public override int CountOfArgs => Arguments.Count;

}