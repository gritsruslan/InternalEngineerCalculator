namespace InternalEngineerCalculator.Main.Functions;

internal sealed class BaseFunction(string name, int countOfArgs, Func<double[], double> function) :
	Function(name)
{
	private readonly Func<double[], double> _function = function;

	public override int CountOfArgs => countOfArgs;

	public override bool IsBaseFunction => true;

	public override double Execute(double[] args) => _function.Invoke(args);
}