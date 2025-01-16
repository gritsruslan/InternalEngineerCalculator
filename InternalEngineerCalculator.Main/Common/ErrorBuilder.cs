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
		new($"Undefined variable \"{name}\" in {GetStackString(callStack)}!");

	public static Error UndefinedFunction(string name, int countOfArgs, ImmutableArray<FunctionInfo> callStack) =>
		new($"Undefined function \"{name}\" with {countOfArgs} args in {GetStackString(callStack)}!");

	private static string GetStackString(ImmutableArray<FunctionInfo> callStack)
	{
		var sb = new StringBuilder();
		for (int i = 0; i < callStack.Length; i++)
		{
			if (i - 1 == callStack.Length)
				sb.Append(callStack);
			else
				sb.Append($"{callStack} in ");
		}
		return sb.ToString();
	}
}