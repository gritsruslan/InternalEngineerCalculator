namespace InternalEngineerCalculator.Main.Functions;

/// <summary> function info used is evaluating process </summary>
internal class FunctionEvaluatingInfo(string name, int countOfArgs, Dictionary<FunctionArgument, double> argsDictionary)
{
	public string Name { get; } = name;

	public int CountOfArgs { get; } = countOfArgs;

	public IReadOnlyDictionary<FunctionArgument, double> ArgsDictionary { get; } = argsDictionary;
}