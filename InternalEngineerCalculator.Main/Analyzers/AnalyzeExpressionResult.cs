using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Functions;

namespace InternalEngineerCalculator.Main.Analyzers;

internal sealed class AnalyzeExpressionResult(ImmutableArray<(ImmutableArray<FunctionInfo>, string)> undefinedVariables,
	ImmutableArray<ImmutableArray<FunctionInfo>> undefinedFunctions)
{
	public ImmutableArray<(ImmutableArray<FunctionInfo>, string)> UndefinedVariables { get; } = undefinedVariables;

	public ImmutableArray<ImmutableArray<FunctionInfo>> UndefinedFunctions { get; } = undefinedFunctions;

	public bool IsSuccess => UndefinedVariables.Length == 0 && UndefinedFunctions.Length == 0;
}