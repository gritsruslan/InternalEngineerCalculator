using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Extensions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

/// <summary> Evaluate ast-tree into number </summary>
public sealed class Evaluator(FunctionManager functionManager, VariableManager variableManager)
{
	// Function call stack with their arguments
	private readonly Stack<FunctionEvaluatingInfo> _functionCallStack = [];

	// reduce call stack to call immut array for convenience
	private ImmutableArray<FunctionInfo> FunctionCallArray => _functionCallStack
		.Select(fi => new FunctionInfo(fi.Name, fi.CountOfArgs))
		.Reverse().ToImmutableArray();

	private bool IsInCustomFunction => _functionCallStack.IsNotEmpty();

	/// <summary> Check function for circular dependency during evaluation </summary>
	// Example :
	// f(x) = 20 + x
	// g(x) = f(x) + 1
	// #override f(x) = g(x) / 42
	// f(x) calls g(x) that calls f(x) => circular dependency, can't evaluate function
	private bool DoesProduceCircularDependency(FunctionInfo funcInfo) =>
		_functionCallStack.Select(calledFunc => new FunctionInfo(calledFunc.Name, calledFunc.CountOfArgs))
			.Any(func => func == funcInfo);


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

		return ue.Type switch
		{
			UnaryExpressionType.Factorial => RMath.Factorial(exprValue),
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
			return ErrorBuilder.DivisionByZero();

		var result = be.OperationType switch
		{
			BinaryOperationType.Addition => left + right,
			BinaryOperationType.Subtraction => left - right,
			BinaryOperationType.Multiplication => left * right,
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
			var currentArgs = _functionCallStack.Peek().ArgsDictionary;
			if (currentArgs.TryGetValue(new FunctionArgument(ve.Name), out var arg))
				return arg;
		}

		var variableResult = variableManager.GetVariableValue(ve.Name);

		if (variableResult.IsFailure)
			return ErrorBuilder.UndefinedVariable(ve.Name, FunctionCallArray);

		return variableResult.Value;
	}

	private Result<double> EvaluateFunctionCallExpression(FunctionCallExpression fe)
	{
		//GetFunction
		var funcResult = functionManager.GetFunction(new FunctionInfo(fe.Name, fe.CountOfArgs));
		if (!funcResult.TryGetValue(out var function))
			return ErrorBuilder.UndefinedFunction(fe.Name, fe.CountOfArgs, FunctionCallArray);

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

	private double EvaluateBaseFunction(BaseFunction bf, ImmutableArray<double> argValues) =>
		bf.Function.Invoke(argValues);

	private Result<double> EvaluateCustomFunction(CustomFunction cf, ImmutableArray<double> args)
	{
		// Check for producing circular dependency (when function calls itself)
		var doesProduceCd = DoesProduceCircularDependency(new FunctionInfo(cf.Name, cf.CountOfArgs));
		if (doesProduceCd)
			return ErrorBuilder.FuncProducesCircularDependency(cf.Name, cf.CountOfArgs);

		// Create function arguments dictionary
		var argsDict = new Dictionary<FunctionArgument, double>(args.Length);
		for (int i = 0; i < args.Length; i++)
			argsDict.Add(cf.Arguments[i], args[i]);

		//Push arguments in call stack
		_functionCallStack.Push(new FunctionEvaluatingInfo(cf.Name, args.Length, argsDict));
		var evalResult = Evaluate(cf.Expression);
		_functionCallStack.Pop();

		return evalResult;
	}
}