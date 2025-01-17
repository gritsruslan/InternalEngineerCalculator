using System.Collections.Immutable;

namespace InternalEngineerCalculator.Main.Functions;

internal sealed class BaseFunction(string name, int countOfArgs, Func<ImmutableArray<double>, double> function) :
	Function(name)
{
	public Func<ImmutableArray<double>, double> Function { get; } = function;

	public override bool IsBaseFunction => true;

	public override int CountOfArgs { get; } = countOfArgs;
}