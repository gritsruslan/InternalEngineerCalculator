using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main.Functions;

internal sealed class FunctionDependencies(
	ICollection<FunctionInfo> usedFunctions,
	ICollection<string> usedVariableNames)
{
	public ICollection<FunctionInfo> UndefinedInsideFunctions => usedFunctions;

	public ICollection<string> UndefinedInsideVariables => usedVariableNames;
}

internal sealed class FunctionDependenciesAnalyzer(
	FunctionAssignmentExpression functionExpression,
	FunctionManager functionManager,
	VariableManager variableManager)
{
	private readonly FunctionAssignmentExpression _functionExpression = functionExpression;

	private ImmutableArray<string> Args => _functionExpression.Args;

	private readonly FunctionManager _functionManager = functionManager;

	private readonly VariableManager _variableManager = variableManager;

	private readonly List<FunctionInfo> _usedFunctions = [];

	private readonly List<string> _usedVariableNames = [];

	public FunctionDependencies FindOutDependencies()
	{
		var expr = _functionExpression.Expression;
		Analyze(expr);
		return new FunctionDependencies(_usedFunctions, _usedVariableNames);
	}

	private void Analyze(Expression expression)
	{
		if (expression is BinaryExpression be)
		{
			Analyze(be.Left);
			Analyze(be.Right);
		}
		else if (expression is UnaryExpression ue)
		{
			Analyze(ue.Expression);
		}
		else if (expression is VariableExpression ve)
		{
			if(Args.Contains(ve.Name))
				return;

			_usedVariableNames.Add(ve.Name);
		}
		else if (expression is FunctionCallExpression fe)
		{
			_usedFunctions.Add(new FunctionInfo(fe.Name, fe.CountOfArgs));
		}
	}
}