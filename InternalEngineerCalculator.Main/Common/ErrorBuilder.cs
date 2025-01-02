using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Common;

internal static class ErrorBuilder
{
	internal static Error EndOfInput() => new("Unexpected end of input");

	internal static Error UnexpectedToken(TokenType received, TokenType expected) =>
		new($"Unexpected operator {received}, expected : {expected}");

	internal static Error UnexpectedToken(string expected, string received) =>
		new($"Unexpected operator {received}, expected : {expected}");

	internal static Error UnexpectedToken() =>
		new("Expected number, function call or open parenthesis or function call!");

	internal static Error FunctionNotFound(string functionName, int countOfArgs) =>
		new($"Cannot find a function \"{functionName}\" with {countOfArgs} needed arguments");
}