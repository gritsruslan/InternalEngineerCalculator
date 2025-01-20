using System.Collections.Immutable;
using System.Text;
using InternalEngineerCalculator.Main.Extensions;
using InternalEngineerCalculator.Main.Functions;

namespace InternalEngineerCalculator.Main.Common;

internal static class ErrorBuilder
{
	public static Error IncorrectExpression() =>
		new("Incorrect expression! Please correct mistake and try again.");

	public static Error FunctionNotFound(string functionName, int countOfArgs) =>
		new($"Cannot find a function \"{functionName}\" with {countOfArgs} needed arguments");

	public static Error UndefinedVariable(string name, ImmutableArray<FunctionInfo> callStack) =>
		new($"Undefined variable \"{name}\" {GetStackString(callStack)}!");

	public static Error DivisionByZero() => new("Division by zero or by too small number!");

	public static Error UndefinedFunction(string name, int countOfArgs, ImmutableArray<FunctionInfo> callStack) =>
		new($"Undefined function \"{name}\" with {countOfArgs} args {GetStackString(callStack)}!");

	public static Error FuncProducesCircularDependency(string name, int countOfArgs) =>
		 new($"Function \"{name}\" with {countOfArgs} args produces circular dependency (function calls itself)! " +
		     $"Override it for correct working.");

	private static string GetStackString(ImmutableArray<FunctionInfo> callStack)
	{
		var sb = new StringBuilder();

		if (callStack.IsEmpty)
			return string.Empty;

		sb.Append("in ");
		for (int i = 0; i < callStack.Length; i++)
		{
			var str = callStack[i].ToPrettyString();
			if (i + 1 == callStack.Length)
				sb.Append(str);
			else
				sb.Append($"{str} in ");
		}

		return sb.ToString();
	}
}