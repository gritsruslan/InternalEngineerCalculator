namespace InternalEngineerCalculator.Main.Functions;

internal sealed class BaseFunction(string name, int countOfArgs, Func<double[], double> function) :
	Function(name)
{
	public Func<double[], double> Function => function;

	public override int CountOfArgs => countOfArgs;

	public override bool IsBaseFunction => true;
}