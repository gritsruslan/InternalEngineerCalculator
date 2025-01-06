using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Tokens;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

internal sealed class Evaluator(FunctionManager functionManager, VariableManager variableManager)
{
	private readonly FunctionManager _functionManager = functionManager;

	private readonly VariableManager _variableManager = variableManager;

	private readonly HashSet<TokenType> _correctMathOperators =
		[TokenType.Plus, TokenType.Minus, TokenType.Divide, TokenType.Multiply, TokenType.Pow, TokenType.Remainder];

	private bool IsCorrectMathExpressionOperator(TokenType operatorType) =>
		_correctMathOperators.Contains(operatorType);

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
			if (!exprResult.TryGetValue(out var expr))
				return exprResult;

			if (ue.Type == UnaryExpressionType.Minus)
				expr *= -1;
			else if (ue.Type == UnaryExpressionType.Factorial)
			{
				var factorialResult = RMath.Factorial(expr);
				if (factorialResult.IsSuccess)
					expr = factorialResult.Value;
				else
					return factorialResult;
			}
			else if (ue.Type == UnaryExpressionType.Module)
			{
				expr = Math.Abs(expr);
			}

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

		if (operation.Type == TokenType.Divide && right == 0)
			return new Error("Divide by zero is not allowed!");

		if (!IsCorrectMathExpressionOperator(operation.Type))
			return new Error("Incorrect math operator!");

		double result = operation.Type switch
		{
			TokenType.Plus => left + right,
			TokenType.Minus => left - right,
			TokenType.Multiply => left * right,
			TokenType.Divide => left/right,
			TokenType.Pow => Math.Pow(left, right),
			TokenType.Remainder => left % right,
			_ => throw new Exception("Incorrect math operator!")
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

		var immutableArrayEvaluatedArgs = evaluatedArgValues.ToImmutableArray();

		if (function is BaseFunction baseFunction)
			return EvaluateBaseFunction(baseFunction, immutableArrayEvaluatedArgs);

		if (function is CustomFunction customFunction)
			return EvaluateCustomFunction(customFunction, immutableArrayEvaluatedArgs);

		throw new Exception("Incorrect function call!");
	}

	private double EvaluateBaseFunction(BaseFunction baseFunction, ImmutableArray<double> args)
		=> baseFunction.Function.Invoke(args);

	private Result<double> EvaluateCustomFunction(CustomFunction customFunction, ImmutableArray<double> args)
	{
		var argumentsDictionary = new Dictionary<FunctionArgument, double>();

		for (int i = 0; i < args.Length; i++)
			argumentsDictionary.Add(customFunction.Arguments[i], args[i]);

		var expression = customFunction.FunctionExpression;

		return Evaluate(expression, true, argumentsDictionary);
	}
}