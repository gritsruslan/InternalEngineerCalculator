namespace InternalEngineerCalculator.Main.Tokens;

public enum TokenType
{
	Number,

	Plus,
	Minus,
	Divide,
	Multiply,
	Pow,
	OpenParenthesis,
	CloseParenthesis,
	EndOfLine,
	Identifier,
	Comma,

	BinaryExpression,
	UnaryOperation,
	FunctionCall
}