using System.Text;
using InternalEngineerCalculator.Main.Analyzers;

namespace InternalEngineerCalculator.Main.Extensions;

internal static class AnalyzeExpressionResultExtensions
{
	public static void PrettyPrint(this AnalyzeExpressionResult result)
	{
		//undefined functions
		foreach (var undefinedFunctionStackCall in result.UndefinedFunctions)
		{
			var undefinedFunction = undefinedFunctionStackCall.Last();
			Console.Write($"Undefined function {undefinedFunction.FunctionName} with {undefinedFunction.CountOfArg} in ");

			for (int i = 0; i < undefinedFunctionStackCall.Length; i++)
			{
				var function = undefinedFunctionStackCall[i];
				if (i == undefinedFunctionStackCall.Length - 1)
				{
					Console.Write(function.ToPrettyString());
					continue;
				}
				Console.Write(function.ToPrettyString() + "->");
			}

			Console.WriteLine();
		}

		//undefined variables
		foreach (var undefinedVariableStackCall in result.UndefinedVariables)
		{
			var undefinedVariable = undefinedVariableStackCall.Item2;
			Console.Write($"Undefined variable {undefinedVariable} in ");

			var undefinedVariableFunctionStackCall = undefinedVariableStackCall.Item1;

			for (int i = 0; i < undefinedVariableFunctionStackCall.Length; i++)
			{
				var function = undefinedVariableFunctionStackCall[i];
				if (i == undefinedVariableFunctionStackCall.Length - 1)
				{
					Console.Write(function.ToPrettyString());
					continue;
				}
				Console.Write(function.ToPrettyString() + " -> ");
			}

			Console.Write(undefinedVariable + "!");
			Console.WriteLine();
		}
	}
}