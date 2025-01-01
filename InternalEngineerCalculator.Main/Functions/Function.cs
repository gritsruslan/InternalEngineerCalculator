namespace InternalEngineerCalculator.Main.Functions;

internal abstract class Function(string name)
{
	public string Name => name;

	public abstract bool IsBaseFunction { get; }

	public abstract int CountOfArgs { get; }

}