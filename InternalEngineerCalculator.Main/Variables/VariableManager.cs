using InternalEngineerCalculator.Main.Exceptions;

namespace InternalEngineerCalculator.Main.Variables;

internal sealed class VariableManager
{
	private readonly Dictionary<string, Variable> _variablesContainer = [];
	internal IReadOnlyDictionary<string, Variable> GetVariables() => _variablesContainer;

	public void InitializeBasicVariables()
	{
		_variablesContainer.Add("pi", new Variable("pi", Math.PI, true));
		_variablesContainer.Add("e", new Variable("e", Math.E, true));
	}

	public void InitializeOrUpdateVariable(string name, double value)
	{
		if (_variablesContainer.TryGetValue(name, out var variable) && variable.IsConstant)
			throw new CalculatorException($"Cannot set new value to a variable\"{name}\", because its constant!");

		_variablesContainer[name] = new Variable(name, value, false);
	}

	public double GetVariableValue(string name)
	{
		if (!_variablesContainer.TryGetValue(name, out var variable))
			throw new CalculatorException($"Variable with name \"{name}\" is not exist!");

		return variable.Value;
	}
}