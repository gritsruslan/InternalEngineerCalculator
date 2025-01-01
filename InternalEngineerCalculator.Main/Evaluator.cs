using InternalEngineerCalculator.Main.Exceptions;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Tokens;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

internal sealed class Evaluator(FunctionManager functionManager, VariableManager variableManager)
{
	private readonly FunctionManager _functionManager = functionManager;

	private readonly VariableManager _variableManager = variableManager;

	public double Evaluate(Expression expression, bool isInCustomFunction = false,
		Dictionary<FunctionArgument, double>? functionArguments = null)
	{
		if (expression is NumberExpression ne)
			return ne.Token.Value;

		if (expression is VariableExpression ve)
		{
			if (isInCustomFunction)
			{
				var maybeFuncArgument = new FunctionArgument(ve.Name);
				if (functionArguments!.TryGetValue(maybeFuncArgument, out var value))
					return value;
			}

			return _variableManager.GetVariableValue(ve.Name);
		}

		if (expression is UnaryExpression ue)
		{
			var expr = Evaluate(ue.Expression, isInCustomFunction, functionArguments);

			if (ue.UnaryOperation.Type == TokenType.Minus)
				expr *= -1;
			// Other unary operators
			return expr;
		}

		if (expression is FunctionCallExpression fce)
			return EvaluateFunction(fce, isInCustomFunction, functionArguments);

		var binExpression = expression as BinaryExpression;

		var left = Evaluate(binExpression!.Left, isInCustomFunction, functionArguments);
		var operation = binExpression.Operation;
		var right = Evaluate(binExpression.Right, isInCustomFunction, functionArguments);

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

	private double EvaluateFunction(FunctionCallExpression functionCallExpression, bool isInCustomFunction = false,
		Dictionary<FunctionArgument, double>? functionArguments = null)
	{
		var header = new FunctionCallHeader(functionCallExpression.Name, functionCallExpression.CountOfArgs);

		var function = _functionManager.GetFunctionByHeader(header);

		double[] evaluatedArgValues = new double[functionCallExpression.CountOfArgs];

		for (int i = 0; i < functionCallExpression.Arguments.Length; i++)
		{
			var argExpression = functionCallExpression.Arguments[i];
			double value = Evaluate(argExpression, isInCustomFunction, functionArguments);
			evaluatedArgValues[i] = value;
		}

		if (function is BaseFunction baseFunction)
			return baseFunction.Function.Invoke(evaluatedArgValues);

		if (function is CustomFunction customFunction)
			return EvaluateCustomFunction(customFunction, evaluatedArgValues);

		throw new Exception("Incorrect function call!");

	}

	private double EvaluateCustomFunction(CustomFunction customFunction, double[] args)
	{
		var argumentsDictionary = new Dictionary<FunctionArgument, double>();

		for (int i = 0; i < args.Length; i++)
			argumentsDictionary.Add(customFunction.Arguments[i], args[i]);

		var expression = customFunction.FunctionExpression;

		return Evaluate(expression, true, argumentsDictionary);
	}
}