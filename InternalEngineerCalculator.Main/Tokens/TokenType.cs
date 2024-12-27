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
	EqualSign,

	BinaryExpression,
	UnaryOperation,
	FunctionCall,
	VariableExpression,
	VariableAssignmentExpression,
	FunctionAssignmentExpression
}