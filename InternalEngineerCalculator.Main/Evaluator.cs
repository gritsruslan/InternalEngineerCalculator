using InternalEngineerCalculator.Main.Common;
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

	public Result<double> Evaluate(Expression expression, bool isInCustomFunction = false,
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
			var exprResult = Evaluate(ue.Expression, isInCustomFunction, functionArguments);
			if (exprResult.TryGetValue(out var expr))
				return exprResult;

			if (ue.UnaryOperation.Type == TokenType.Minus)
				expr *= -1;
			// Other unary operators
			return expr;
		}

		if (expression is FunctionCallExpression fce)
			return EvaluateFunction(fce, isInCustomFunction, functionArguments);

		var binExpression = expression as BinaryExpression;

		var leftResult = Evaluate(binExpression!.Left, isInCustomFunction, functionArguments);
		if (!leftResult.TryGetValue(out var left))
			return leftResult;

		var operation = binExpression.Operation;

		var rightResult = Evaluate(binExpression.Right, isInCustomFunction, functionArguments);
		if (!rightResult.TryGetValue(out var right))
			return rightResult;

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

	private Result<double> EvaluateFunction(FunctionCallExpression functionCallExpression, bool isInCustomFunction = false,
		Dictionary<FunctionArgument, double>? functionArguments = null)
	{
		var header = new FunctionCallHeader(functionCallExpression.Name, functionCallExpression.CountOfArgs);

		var functionGetResult = _functionManager.GetFunctionByHeader(header);
		if (!functionGetResult.TryGetValue(out var function))
			return functionGetResult.Error;

		double[] evaluatedArgValues = new double[functionCallExpression.CountOfArgs];

		for (int i = 0; i < functionCallExpression.Arguments.Length; i++)
		{
			var argExpression = functionCallExpression.Arguments[i];

			var valueResult = Evaluate(argExpression, isInCustomFunction, functionArguments);
			if (!valueResult.TryGetValue(out var value))
				return valueResult;

			evaluatedArgValues[i] = value;
		}

		if (function is BaseFunction baseFunction)
			return baseFunction.Function.Invoke(evaluatedArgValues);

		if (function is CustomFunction customFunction)
			return EvaluateCustomFunction(customFunction, evaluatedArgValues);

		throw new Exception("Incorrect function call!");
	}

	private Result<double> EvaluateCustomFunction(CustomFunction customFunction, double[] args)
	{
		var argumentsDictionary = new Dictionary<FunctionArgument, double>();

		for (int i = 0; i < args.Length; i++)
			argumentsDictionary.Add(customFunction.Arguments[i], args[i]);

		var expression = customFunction.FunctionExpression;

		return Evaluate(expression, true, argumentsDictionary);
	}
}