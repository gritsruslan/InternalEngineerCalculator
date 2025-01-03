using System.Collections.Immutable;

namespace InternalEngineerCalculator.Main.Functions;

internal sealed class BaseFunction(string name, int countOfArgs, Func<ImmutableArray<double>, double> function) :
	Function(name)
{
	public Func<ImmutableArray<double>, double> Function => function;

	public override int CountOfArgs => countOfArgs;

	public override bool IsBaseFunction => true;
}