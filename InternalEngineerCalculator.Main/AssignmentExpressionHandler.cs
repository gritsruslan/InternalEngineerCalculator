using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

internal class AssignmentExpressionHandler
{
	private Evaluator _evaluator;

	private VariableManager _variableManager;

	public AssignmentExpressionHandler(Evaluator evaluator, VariableManager variableManager)
	{
		_variableManager = variableManager;
		_evaluator = evaluator;
	}

	public double HandleVariableAssignmentExpression(VariableAssignmentExpression variableExpression)
	{
		var variableName = variableExpression.VariableName;

		var variableValue = _evaluator.Evaluate(variableExpression.VariableValueExpression);

		_variableManager.InitializeOrUpdateVariable(variableName, variableValue);

		return variableValue; // returns new variable value for print
	}
}