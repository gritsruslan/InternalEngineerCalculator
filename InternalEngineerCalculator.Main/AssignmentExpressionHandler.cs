using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

/// <summary> functions and variables assignment handler </summary>
public sealed class AssignmentExpressionHandler(
	Evaluator evaluator,
	VariableManager variableManager,
	FunctionManager functionManager)
{
	public bool HandleFunctionAssignmentExpression(FunctionAssignmentExpression functionExpression) =>
		functionManager.CreateOrOverrideCustomFunction(
			functionExpression.Name,
			functionExpression.Args,
			functionExpression.Expression);

	public Result<double> HandleVariableAssignmentExpression(VariableAssignmentExpression variableExpression)
	{
		var variableName = variableExpression.Name;

		var variableValueResult = evaluator.Evaluate(variableExpression.Expression);
		if (!variableValueResult.TryGetValue(out var variableValue))
			return variableValueResult;

		var result = variableManager.InitializeOrUpdateVariable(variableName, variableValue);
		if (result.IsFailure)
			return result.Error;

		return variableValue; // returns new variable value for print
	}
}