using System.Text;
using InternalEngineerCalculator.Main.Analyzers;

namespace InternalEngineerCalculator.Main.Extensions;

internal static class AnalyzeExpressionResultExtensions
{
	public static string Print(this AnalyzeExpressionResult result)
	{
		var sb = new StringBuilder();

		//undefined functions
		foreach (var undefinedFunctionStackCall in result.UndefinedFunctions)
		{
			var undefinedFunction = undefinedFunctionStackCall.Last();
			sb.Append($"Undefined function {undefinedFunction.FunctionName} with {undefinedFunction.CountOfArg} in ");

			for (int i = 0; i < undefinedFunctionStackCall.Length; i++)
			{
				var function = undefinedFunctionStackCall[i];
				if (i == undefinedFunctionStackCall.Length - 1)
				{
					sb.Append(function.ToPrettyString());
					continue;
				}
				sb.Append(function.ToPrettyString() + "->");
			}

			sb.AppendLine();
		}

		//undefined variables
		foreach (var undefinedVariableStackCall in result.UndefinedVariables)
		{
			var undefinedVariable = undefinedVariableStackCall.Item2;
			sb.Append($"Undefined variable {undefinedVariable} in ");

			var undefinedVariableFunctionStackCall = undefinedVariableStackCall.Item1;

			for (int i = 0; i < undefinedVariableFunctionStackCall.Length; i++)
			{
				var function = undefinedVariableFunctionStackCall[i];
				if (i == undefinedVariableFunctionStackCall.Length - 1)
				{
					sb.Append(function.ToPrettyString());
					continue;
				}
				sb.Append(function.ToPrettyString() + "->");
			}

			sb.Append(undefinedVariable + "!");
			sb.AppendLine();
		}

		return sb.ToString();
	}
}