using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Exceptions;

internal sealed class UnexpectedTokenException : CalculatorException
{
	public UnexpectedTokenException(string message) : base(message){}

	public UnexpectedTokenException(string received, string expected) : base(
		$"Unexpected operator {received}, expected : {expected}") {}

	public UnexpectedTokenException(TokenType received, TokenType expected) : base(
		$"Unexpected operator {received}, expected : {expected}") {}

	public UnexpectedTokenException() : base("Expected number, function call or open parenthesis or function call!") {}
}