using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Extensions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

internal sealed class Evaluator(FunctionManager functionManager, VariableManager variableManager)
{
	private readonly FunctionManager _functionManager = functionManager;

	private readonly VariableManager _variableManager = variableManager;

	// Stack of arguments of called custom functions
	private readonly Stack<Dictionary<FunctionArgument, double>> _customFuncArgumentsStack = [];

	private bool IsInCustomFunction => _customFuncArgumentsStack.IsEmpty();

	public Result<double> Evaluate(Expression expression)
	{
		return expression switch
		{
			NumberExpression ne => ne.Value,
			UnaryExpression ue => EvaluateUnaryExpression(ue),
			BinaryExpression be => EvaluateBinaryExpression(be),
			VariableExpression ve => EvaluateVariableExpression(ve),
			FunctionCallExpression fe => EvaluateFunctionCallExpression(fe),
			_ => throw new ArgumentException("Unknown expression to evaluate!")
		};
	}

	private Result<double> EvaluateUnaryExpression(UnaryExpression ue)
	{
		var exprValueResult = Evaluate(ue.Expression);

		if(!exprValueResult.TryGetValue(out var exprValue))
			return exprValueResult;

		if (ue.Type is UnaryExpressionType.Factorial)
			return RMath.Factorial(exprValue);

		return ue.Type switch
		{
			UnaryExpressionType.Minus => -exprValue,
			UnaryExpressionType.Module => Math.Abs(exprValue),
			_ => throw new ArgumentException("Unknown unary expression operator!")
		};
	}

	private Result<double> EvaluateBinaryExpression(BinaryExpression be)
	{
		var leftResult = Evaluate(be.Left);
		if (!leftResult.TryGetValue(out var left))
			return leftResult;

		var rightResult = Evaluate(be.Right);
		if (!rightResult.TryGetValue(out var right))
			return rightResult;

		if (be.OperationType is BinaryOperationType.Division or BinaryOperationType.Remainder && right == 0)
			return new Error("Divizion by zero is not allowed!"); //TODO

		var result = be.OperationType switch
		{
			BinaryOperationType.Addition => left + right,
			BinaryOperationType.Subtraction => left - right,
			BinaryOperationType.Multiplication => left / right,
			BinaryOperationType.Division => left / right,
			BinaryOperationType.Remainder => left % right,
			BinaryOperationType.Power => Math.Pow(left, right),
			_ => throw new ArgumentException("Unknown binary operation!")
		};

		return result;
	}

	private Result<double> EvaluateVariableExpression(VariableExpression ve)
	{
		if (IsInCustomFunction)
		{
			var currentArgs = _customFuncArgumentsStack.Peek();
			if (currentArgs.TryGetValue(new FunctionArgument(ve.Name), out var arg))
				return arg;
		}

		return _variableManager.GetVariableValue(ve.Name);
	}

	private Result<double> EvaluateFunctionCallExpression(FunctionCallExpression fe)
	{
		//GetFunction
		var funcResult = _functionManager.GetFunction(new FunctionInfo(fe.Name, fe.CountOfArgs));
		if (!funcResult.TryGetValue(out var function))
			return funcResult.Error;

		//Evaluate Arguments
		var argsArray = new List<double>(fe.CountOfArgs);
		foreach (var argExpr in fe.Arguments)
		{
			var argValueResult = Evaluate(argExpr);
			if (!argValueResult.TryGetValue(out var value))
				return argValueResult;

			argsArray.Add(value);
		}

		var argsImmut = argsArray.ToImmutableArray();

		if (function is BaseFunction bf)
			return EvaluateBaseFunction(bf, argsImmut);

		if (function is CustomFunction cf)
			return EvaluateCustomFunction(cf, argsImmut);

		throw new Exception("Unknown function type!");
	}

	private Result<double> EvaluateBaseFunction(BaseFunction bf, ImmutableArray<double> argValues) =>
		bf.Function.Invoke(argValues);

	private Result<double> EvaluateCustomFunction(CustomFunction cf, ImmutableArray<double> args)
	{
		var argsDict = new Dictionary<FunctionArgument, double>(args.Length);

		for (int i = 0; i < args.Length; i++)
			argsDict.Add(cf.Arguments[i], args[i]);

		_customFuncArgumentsStack.Push(argsDict);
		var evalResult = Evaluate(cf.Expression);
		_customFuncArgumentsStack.Pop();

		return evalResult;
	}
}