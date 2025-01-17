using System.Collections.Immutable;
using System.Text;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Common;

internal static class ErrorBuilder
{
	public static Error EndOfInput() => new("Unexpected end of input");

	public static Error UnexpectedToken(TokenType received, TokenType expected) =>
		new($"Unexpected operator {received}, expected : {expected}");

	public static Error UnexpectedToken(string expected, string received) =>
		new($"Unexpected operator {received}, expected : {expected}");

	public static Error UnexpectedToken() =>
		new("Expected number, function call or open parenthesis or function call!");

	public static Error FunctionNotFound(string functionName, int countOfArgs) =>
		new($"Cannot find a function \"{functionName}\" with {countOfArgs} needed arguments");

	public static Error UndefinedVariable(string name, ImmutableArray<FunctionInfo> callStack) =>
		new($"Undefined variable \"{name}\" {GetStackString(callStack)}!");

	public static Error UndefinedFunction(string name, int countOfArgs, ImmutableArray<FunctionInfo> callStack) =>
		new($"Undefined function \"{name}\" with {countOfArgs} args {GetStackString(callStack)}!");

	public static Error FuncProducesCircularDependency(string name, int countOfArgs) =>
		 new($"Function \"{name}\" with {countOfArgs} args produces circular dependency (Function calls itself)! " +
		     $"Override it for correct working.");

	private static string GetStackString(ImmutableArray<FunctionInfo> callStack)
	{
		var sb = new StringBuilder();

		if (callStack.IsEmpty)
			return string.Empty;

		sb.Append("in ");
		for (int i = 0; i < callStack.Length; i++)
		{
			var str = $"{callStack[i].Name}<{callStack[i].CountOfArg}>";
			if (i + 1 == callStack.Length)
				sb.Append(str);
			else
				sb.Append($"{str} in ");
		}

		return sb.ToString();
	}
}