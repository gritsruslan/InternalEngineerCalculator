namespace InternalEngineerCalculator.Main.Functions;

public abstract class Function(string name)
{
	public string Name => name;

	public abstract bool IsBaseFunction { get; }

	public abstract int CountOfArgs { get; }
}