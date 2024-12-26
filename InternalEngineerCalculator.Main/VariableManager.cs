namespace InternalEngineerCalculator.Main;


internal record Variable(string Name, double Value, bool IsBasic);

internal sealed class VariableManager
{
	private Dictionary<string, Variable> _variablesContainer = [];

	public void InitializeBasicVariables()
	{
		_variablesContainer.Add("pi", new Variable("pi", Math.PI, true));
		_variablesContainer.Add("e", new Variable("e", Math.E, true));
	}

	public void InitializeNewCustomVariable(string name, double value)
	{
		_variablesContainer[name] = new Variable(name, value, false);
	}
}