using InternalEngineerCalculator.Main.Common;

namespace InternalEngineerCalculator.Main.Variables;

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

	public bool HasVariable(string name) => _variablesContainer.ContainsKey(name);

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

	public bool DeleteVariable(string name)
	{
		return _variablesContainer.Remove(name);
	}
}