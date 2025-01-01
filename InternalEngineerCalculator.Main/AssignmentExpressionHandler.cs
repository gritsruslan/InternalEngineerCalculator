using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

internal class AssignmentExpressionHandler(
	Evaluator evaluator,
	VariableManager variableManager,
	FunctionManager functionManager)
{
	private readonly Evaluator _evaluator = evaluator;

	private readonly VariableManager _variableManager = variableManager;

	private readonly FunctionManager _functionManager = functionManager;

	public void HandleFunctionAssignmentExpression(FunctionAssignmentExpression functionExpression)
	{
		_functionManager.CreateNewCustomFunction(
			functionExpression.Name,
			functionExpression.Args,
			functionExpression.Expression);
	}

	public double HandleVariableAssignmentExpression(VariableAssignmentExpression variableExpression)
	{
		var variableName = variableExpression.Name;

		var variableValue = _evaluator.Evaluate(variableExpression.Expression);

		_variableManager.InitializeOrUpdateVariable(variableName, variableValue);

		return variableValue; // returns new variable value for print
	}
}