using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

internal sealed class AssignmentExpressionHandler(
	Evaluator evaluator,
	VariableManager variableManager,
	FunctionManager functionManager)
{
	private readonly Evaluator _evaluator = evaluator;

	private readonly VariableManager _variableManager = variableManager;

	private readonly FunctionManager _functionManager = functionManager;

	public EmptyResult HandleFunctionAssignmentExpression(FunctionAssignmentExpression functionExpression)
	{
		return _functionManager.CreateNewCustomFunction(
			functionExpression.Name,
			functionExpression.Args,
			functionExpression.Expression);
	}

	public Result<double> HandleVariableAssignmentExpression(VariableAssignmentExpression variableExpression)
	{
		var variableName = variableExpression.Name;

		var variableValueResult = _evaluator.Evaluate(variableExpression.Expression);
		if (variableValueResult.TryGetValue(out var variableValue))
			return variableValueResult;

		_variableManager.InitializeOrUpdateVariable(variableName, variableValue);

		return variableValue; // returns new variable value for print
	}
}