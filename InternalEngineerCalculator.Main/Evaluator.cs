using InternalEngineerCalculator.Main.Exceptions;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Tokens;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

internal sealed class Evaluator
{
	private readonly FunctionManager _functionManager;

	private readonly VariableManager _variableManager;

	public Evaluator(FunctionManager functionManager, VariableManager variableManager)
	{
		_functionManager = functionManager;
		_variableManager = variableManager;
	}

	public double Evaluate(Expression expression)
	{
		if (expression is NumberExpression ne)
			return ne.Token.Value;

		if (expression is VariableExpression ve)
			return _variableManager.GetVariableValue(ve.Name);

		if (expression is UnaryExpression ue)
		{
			var expr = Evaluate(ue.Expression);

			if (ue.UnaryOperation.Type == TokenType.Minus)
				expr *= -1;
			// Other unary operators
			return expr;
		}

		if (expression is FunctionCallExpression fce)
			return EvaluateFunction(fce);

		var binExpression = expression as BinaryExpression;

		var left = Evaluate(binExpression!.Left);
		var operation = binExpression.Operation;
		var right = Evaluate(binExpression.Right);

		double result = operation.Type switch
		{
			TokenType.Plus => left + right,
			TokenType.Minus => left - right,
			TokenType.Multiply => left * right,
			TokenType.Divide => right == 0 ? throw new CalculatorDivideByZeroException() : left/right,
			TokenType.Pow => Math.Pow(left, right),
			_ => throw new CalculatorException("Unknown math operator!")
		};

		return result;
	}

	private double EvaluateFunction(FunctionCallExpression functionCallExpression)
	{
		var header = new FunctionCallHeader(functionCallExpression.Name, functionCallExpression.CountOfArgs);

		var function = _functionManager.GetFunctionByHeader(header);

		double[] evaluatedArgValues = new double[functionCallExpression.CountOfArgs];

		for (int i = 0; i < functionCallExpression.Arguments.Length; i++)
		{
			var argExpression = functionCallExpression.Arguments[i];
			double value = Evaluate(argExpression);
			evaluatedArgValues[i] = value;
		}

		return function.Execute(evaluatedArgValues);
	}
}