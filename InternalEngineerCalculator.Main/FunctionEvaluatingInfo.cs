using InternalEngineerCalculator.Main.Functions;

namespace InternalEngineerCalculator.Main;

internal class FunctionEvaluatingInfo(string name, int countOfArgs, Dictionary<FunctionArgument, double> argsDictionary)
{
	public string Name { get; } = name;

	public int CountOfArgs { get; } = countOfArgs;

	public IReadOnlyDictionary<FunctionArgument, double> ArgsDictionary { get; } = argsDictionary;
}