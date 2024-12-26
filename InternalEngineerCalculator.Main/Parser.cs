using InternalEngineerCalculator.Main.Exceptions;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main;

internal sealed class Parser
{
	private List<Token> _tokens;

	private int _position;

	private Token? Current => _position < _tokens.Count ? _tokens[_position] : null;

	private Token? NextToken => _position + 1 < _tokens.Count ? _tokens[_position + 1] : null;

	private void Next() => _position++;
	public Parser(List<Token> tokens)
	{
		_tokens = tokens;
	}

	public static bool IsAssignmentExpression(List<Token> tokens) => tokens.Any(t => t.Type == TokenType.EqualSign);

	public Expression ParseExpression(int parentPrecedence = 0, bool isInFunction = false)
	{
		Expression left;
		if (Current is null)
			throw new UnexpectedTokenException();

		// if its unary expression (like -5 or -9)
		var unaryOperatorPrecedence = GetUnaryOperatorPrecedence(Current);
		if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
		{
			var operatorToken = Current as NonValueToken;
			Next();
			var operand = ParseExpression(unaryOperatorPrecedence);
			left = new UnaryExpression(operatorToken!, operand);
		}
		//if has no unary operator
		else
		{
			if (Current.Type == TokenType.Identifier)
			{
				if (NextToken?.Type == TokenType.OpenParenthesis)
					left = ParseFunction();
				else
					left = ParseVariable();
			}
			else
			{
				left = ParseParenthesis();
			}
		}

		while (true)
		{
			if(Current is null)
				break;

			// break if current token is function separator and its in function right now
			if(isInFunction && Current.Type is TokenType.Comma or TokenType.CloseParenthesis)
				break;

			if (Current is not NonValueToken operatorToken)
				throw new UnexpectedTokenException(Current.Type.ToString(), "math operator");

			var precedence = GetOperationPrecedence(operatorToken);

			// if current operator precedence less than parent precedence => break
			if (precedence == 0 || precedence <= parentPrecedence)
				break;

			Next();

			// parse second operand
			var right = ParseExpression(precedence, isInFunction);
			left = new BinaryExpression(left, operatorToken, right);
		}

		return left;
	}

	public VariableAssignmentExpression ParseAssignmentExpression()
	{
		if (Current!.Type != TokenType.Identifier)
			throw new CalculatorException("Incorrect variable assignment expression!");

		var variableIdentifierToken = Current;

		Next();

		if (Current is null || Current.Type != TokenType.EqualSign)
			throw new CalculatorException(
				"Expected equal sign token after variable name in variable assignment expression!");

		Next();

		var variableExpression = ParseExpression();

		return new VariableAssignmentExpression(variableIdentifierToken, variableExpression);
	}

	private VariableExpression ParseVariable()
	{
		var identifierToken = Current;
		Next();
		return new VariableExpression(identifierToken!);
	}

	private FunctionCallExpression ParseFunction()
	{
		var functionName = Current!.ValueString;

		// skip function name token and open parenthesis token
		Next();

		if (Current is null || Current.Type != TokenType.OpenParenthesis)
			throw new CalculatorException("Expected open parenthesis token in start of a function call!");

		Next();

		if (Current is null)
			throw new EndOfInputException();
		if (Current.Type == TokenType.CloseParenthesis)
			throw new CalculatorException("Function cannot have zero arguments!");

		List<Expression> expressions = new List<Expression>();

		// while the function is finished parsing its arguments into expressions
		while (Current.Type != TokenType.CloseParenthesis)
		{
			expressions.Add(ParseExpression(isInFunction: true));

			if (Current is null)
				throw new CalculatorException("Function call must have close parenthesis!");

			if (Current.Type != TokenType.Comma && Current.Type != TokenType.CloseParenthesis)
				throw new CalculatorException("Expected close parenthesis or comma in function call!");

			if(Current.Type == TokenType.Comma)
				Next(); // skip comma

			if (Current is null)
				throw new EndOfInputException();
		}

		Next(); // skip close parenthesis
		return new FunctionCallExpression(functionName, expressions.ToArray());
	}

	private static int GetUnaryOperatorPrecedence(Token token)
	{
		return token.Type switch
		{
			TokenType.Plus => 3,
			TokenType.Minus => 3,
			_ => 0
		};
	}
	private static int GetOperationPrecedence(NonValueToken token)
	{
		return token.Type switch
		{
			TokenType.Plus => 1,
			TokenType.Minus => 1,

			TokenType.Multiply => 2,
			TokenType.Divide => 2,

			TokenType.Pow => 3,

			_ => 0
		};
	}

	private Expression ParseParenthesis()
	{
		if (Current == null)
			throw new EndOfInputException();

		Expression expr;
		// if current token is open parenthesis - parse in parenthesis expression first of all
		if (Current.Type == TokenType.OpenParenthesis)
		{
			Next();
			// parse in parenthesis expression
			expr = ParseExpression();

			if (Current == null)
				throw new EndOfInputException();

			if (Current.Type != TokenType.CloseParenthesis)
				throw new UnexpectedTokenException(Current.Type,TokenType.CloseParenthesis );

			Next();
			return expr; // return in parenthesises expression as a single whole
		}

		// if its not open parenthesis token than its must be a number!
		if (Current is not NumberToken numberToken)
			throw new UnexpectedTokenException();

		Next();

		expr = new NumberExpression(numberToken);
		return expr;
	}
}