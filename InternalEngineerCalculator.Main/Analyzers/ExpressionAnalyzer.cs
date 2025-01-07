using System.Collections.Immutable;
using System.Diagnostics;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main.Analyzers;

internal sealed class ExpressionAnalyzer(FunctionManager functionManager, VariableManager variableManager, Expression expression)
{
	private readonly Expression _expression = expression;

	private readonly FunctionManager _functionManager = functionManager;

	private readonly VariableManager _variableManager = variableManager;

	private readonly List<(List<FunctionInfo>, string)> _undefinedVariables = [];

	private readonly List<List<FunctionInfo>> _undefinedFunctions = [];


	public AnalyzeExpressionResult Analyze()
	{
		Analyze(new Stack<FunctionInfo>(), _expression);

		var undefinedVariables =
			_undefinedVariables.Select(tuple => (tuple.Item1.ToImmutableArray(), tuple.Item2)).ToImmutableArray();

		var undefinedFunctions =
			_undefinedFunctions.Select(l => l.ToImmutableArray()).ToImmutableArray();

		return new AnalyzeExpressionResult(undefinedVariables, undefinedFunctions);
	}

	private void Analyze(Stack<FunctionInfo> functionCallStack, Expression currentExpression)
	{
		if (currentExpression is BinaryExpression be)
		{
			Analyze(functionCallStack, be.Left);
			Analyze(functionCallStack, be.Right);
		}
		else if (currentExpression is UnaryExpression ue)
		{
			Analyze(functionCallStack, currentExpression);
		}
		else if (currentExpression is FunctionCallExpression fe)
		{
			foreach (var funcArg in fe.Arguments)
				Analyze(functionCallStack, funcArg);

			var funcInfo = new FunctionInfo(fe.Name, fe.CountOfArgs);
			var functionGetResult = _functionManager.GetFunctionByHeader(funcInfo);

			var functionCallStackList = functionCallStack.ToList();
			functionCallStackList.Add(funcInfo);

			if (!functionGetResult.TryGetValue(out var function))
			{
				_undefinedFunctions.Add(functionCallStackList);
				return;
			}

			if(function is BaseFunction)
				return;

			var customFunction = function as CustomFunction;
			functionCallStack.Push(funcInfo);
			Analyze(functionCallStack, customFunction!.FunctionExpression);
			functionCallStack.Pop();
		}
		else if (currentExpression is VariableExpression ve)
		{
			if (functionCallStack.Count != 0)
			{
				var currentFunction = _functionManager
					.GetFunctionByHeader(functionCallStack.Peek()).Value as CustomFunction;

				if(currentFunction!.Arguments.Contains(new FunctionArgument(ve.Name)))
					return;
			}

			if (!_variableManager.HasVariable(ve.Name))
			{
				var functionCallsToVariableStack = functionCallStack.ToList();
				_undefinedVariables.Add((functionCallsToVariableStack, ve.Name));
			}
		}
	}
}