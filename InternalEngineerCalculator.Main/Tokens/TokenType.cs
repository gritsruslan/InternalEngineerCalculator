namespace InternalEngineerCalculator.Main;

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

	BinaryExpression,
	UnaryOperation
}