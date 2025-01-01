using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

internal class AssignmentExpressionHandler
{
	private Evaluator _evaluator;

	private VariableManager _variableManager;

	private FunctionManager _functionManager;

	public AssignmentExpressionHandler(Evaluator evaluator, VariableManager variableManager, FunctionManager functionManager)
	{
		_variableManager = variableManager;
		_functionManager = functionManager;
		_evaluator = evaluator;
	}

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