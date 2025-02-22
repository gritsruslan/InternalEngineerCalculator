using InternalEngineerCalculator.Main.Common;

namespace InternalEngineerCalculator.Main.Variables;

/// <summary> Container for functions </summary>
public sealed class VariableManager
{
	private readonly Dictionary<string, Variable> _variablesContainer = [];

	internal IReadOnlyDictionary<string, Variable> GetVariables() => _variablesContainer;

	public void InitializeBasicVariables()
	{
		_variablesContainer.Add("pi", new Variable("pi", Math.PI, true));
		_variablesContainer.Add("e", new Variable("e", Math.E, true));
		_variablesContainer.Add("tau", new Variable("e", Math.Tau, true));
	}

	public EmptyResult InitializeOrUpdateVariable(string name, double value)
	{
		if (_variablesContainer.TryGetValue(name, out var variable) && variable.IsConstant)
			return new Error($"Cannot set new value to a variable\"{name}\", because its constant!");

		_variablesContainer[name] = new Variable(name, value, false);
		return EmptyResult.Success();
	}

	public Result<double> GetVariableValue(string name)
	{
		if (!_variablesContainer.TryGetValue(name, out var variable))
			return new Error($"Variable with name \"{name}\" is not exist!");

		return variable.Value;
	}


	public EmptyResult DeleteVariable(string name)
	{
		if (!_variablesContainer.TryGetValue(name, out var variable))
			return new Error($"There are no variable with name \"{name}\"");

		if (variable.IsConstant)
			return new Error($"Cannot delete a constant variable \"{name}\"!");

		_variablesContainer.Remove(name);

		return EmptyResult.Success();
	}
}