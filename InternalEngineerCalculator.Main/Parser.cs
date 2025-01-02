using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main;

internal sealed class Parser(ImmutableArray<Token> tokens)
{
	private readonly ImmutableArray<Token> _tokens = tokens;

	private readonly Token _emptyToken = new NonValueToken(TokenType.EndOfLine, string.Empty);

	private int _position;

	private Token Current => _position < _tokens.Length ? _tokens[_position] : _emptyToken;

	private Token NextToken => _position + 1 < _tokens.Length ? _tokens[_position + 1] : _emptyToken;

	private void Next() => _position++;

	private void Next(int offset) => _position += offset;

	private bool IsVariableAssignmentExpression() =>
		Current.Type == TokenType.Identifier && NextToken.Type != TokenType.OpenParenthesis;

	private bool IsFunctionAssignmentExpression() =>
		Current.Type == TokenType.Identifier && NextToken.Type == TokenType.OpenParenthesis;

	public static bool IsAssignmentExpression(ICollection<Token> tokens) =>
		tokens.Any(t => t.Type == TokenType.EqualSign);

	public Result<Expression> ParseExpression(int parentPrecedence = 0, bool isInFunction = false)
	{
		Result<Expression> leftResult;
		if (Current.Type == TokenType.EndOfLine)
			return ErrorBuilder.UnexpectedToken();

		// if its unary expression (like -5 or -9)
		var unaryOperatorPrecedence = GetUnaryOperationPrecedence(Current);
		if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
		{
			var operatorToken = Current as NonValueToken;
			Next();
			var operandExpression = ParseExpression(unaryOperatorPrecedence);
			if (!operandExpression.TryGetValue(out var operand))
				return operandExpression.Error;
			leftResult = new UnaryExpression(operatorToken!, operand);
		}
		//if it has no unary operator
		else
		{
			if (Current.Type is TokenType.Identifier && NextToken.Type == TokenType.OpenParenthesis)
				leftResult = ParseFunction();
			else if (Current.Type is TokenType.Identifier)
				leftResult = ParseVariable();
			else
				leftResult = ParseParenthesis();
		}

		if (!leftResult.TryGetValue(out var left))
			return leftResult.Error;

		while (true)
		{
			if(Current.Type == TokenType.EndOfLine)
				break;

			// break if current token is function separator and its in function right now
			if(isInFunction && Current.Type is TokenType.Comma or TokenType.CloseParenthesis)
				break;

			if (Current is not NonValueToken operatorToken)
				return ErrorBuilder.UnexpectedToken(Current.Type.ToString(), "math operator");

			var precedence = GetOperationPrecedence(operatorToken);

			// if current operator precedence less than parent precedence => break
			if (precedence == 0 || precedence <= parentPrecedence)
				break;

			Next();

			// parse second operand
			var rightParseExpression = ParseExpression(precedence, isInFunction);
			if (!rightParseExpression.TryGetValue(out var right))
				return rightParseExpression.Error;

			left = new BinaryExpression(left, operatorToken, right);
		}

		return left;
	}

	public Result<AssignmentExpression> ParseAssignmentExpression()
	{
		Result<AssignmentExpression> expressionResult;

		if (IsVariableAssignmentExpression())
			expressionResult = ParseVariableAssignmentExpression();
		else if (IsFunctionAssignmentExpression())
			expressionResult = ParseFunctionAssignmentExpression();
		else
			throw new Exception("Incorrect assignment expression!");

		return expressionResult;
	}

	private Result<AssignmentExpression> ParseVariableAssignmentExpression()
	{
		if (Current.Type != TokenType.Identifier)
			return new Error("Incorrect variable assignment expression!");

		var variableIdentifierToken = Current;

		Next();

		if (Current.Type != TokenType.EqualSign)
			return new Error("Expected equal sign token after variable name in variable assignment expression!");

		Next();

		var variableExpressionResult = ParseExpression();
		if (!variableExpressionResult.TryGetValue(out var variableExpression))
			return variableExpressionResult.Error;

		return new VariableAssignmentExpression(variableIdentifierToken, variableExpression);
	}

	private Result<AssignmentExpression> ParseFunctionAssignmentExpression()
	{
		var functionName = Current.ValueString;

		var args = new List<string>();

		Next(2);

		if (Current.Type == TokenType.CloseParenthesis)
			return new Error("Function must take at least 1 argument!");

		while (Current.Type != TokenType.EndOfLine && Current.Type != TokenType.CloseParenthesis)
		{
			args.Add(Current.ValueString);

			Next();

			if (Current.Type == TokenType.EndOfLine)
				return new Error("Function declaration must have close parenthesis!");

			if(Current.Type != TokenType.Comma && Current.Type != TokenType.CloseParenthesis)
				return new Error("Expected close parenthesis or comma in function declaration!");

			if(Current.Type == TokenType.Comma)
				Next();

			if (Current.Type == TokenType.EndOfLine)
				return new Error("Expected close parenthesis in function declaration!");
		}

		Next();

		if (Current.Type != TokenType.EqualSign)
			return new Error("Expected equal sign after header in function declaration");

		Next();

		if (Current.Type == TokenType.EndOfLine)
			return new Error("Function cant be empty!");

		var functionExpressionResult = ParseExpression();
		if (!functionExpressionResult.TryGetValue(out var functionExpression))
			return functionExpressionResult.Error;

		return new FunctionAssignmentExpression(functionName, [..args], functionExpression);
	}

	private VariableExpression ParseVariable()
	{
		var identifierToken = Current;
		Next();
		return new VariableExpression(identifierToken);
	}

	private Result<Expression> ParseFunction()
	{
		var functionName = Current.ValueString;

		// skip function name token and open parenthesis token
		Next(2);

		if (Current.Type == TokenType.EndOfLine)
			return ErrorBuilder.EndOfInput();
		if (Current.Type == TokenType.CloseParenthesis)
			return new Error("Function cannot have zero arguments!");

		var expressions = new List<Expression>();

		// while the function is finished parsing its arguments into expressions
		while (Current.Type != TokenType.CloseParenthesis)
		{
			var parseResult = ParseExpression(isInFunction: true);
			if (!parseResult.TryGetValue(out var expression))
				return parseResult.Error;
			expressions.Add(expression);

			if (Current.Type == TokenType.EndOfLine)
				return new Error("Function call must have close parenthesis!");

			if (Current.Type != TokenType.Comma && Current.Type != TokenType.CloseParenthesis)
				return new Error("Expected close parenthesis or comma in function call!");

			if(Current.Type == TokenType.Comma)
				Next(); // skip comma

			if (Current.Type == TokenType.EndOfLine)
				return ErrorBuilder.EndOfInput();
		}

		Next(); // skip close parenthesis
		return new FunctionCallExpression(functionName, [..expressions]);
	}

	private static int GetUnaryOperationPrecedence(Token token)
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

	private Result<Expression> ParseParenthesis()
	{
		if (Current.Type == TokenType.EndOfLine)
			return ErrorBuilder.EndOfInput();

		Expression expr;
		// if current token is open parenthesis - parse in parentheses expression first of all
		if (Current.Type == TokenType.OpenParenthesis)
		{
			Next();
			// parse in parenthesis expression
			var expressionResult = ParseExpression();
			if (expressionResult.IsFailure)
				return expressionResult.Error;

			expr = expressionResult.Value;

			if (Current.Type == TokenType.EndOfLine)
				return ErrorBuilder.EndOfInput();

			if (Current.Type != TokenType.CloseParenthesis)
				return ErrorBuilder.UnexpectedToken(Current.Type, TokenType.CloseParenthesis);

			Next();
			return expr; // return in parentheses expression as a single whole
		}

		// if its not open parenthesis token than its must be a number!
		if (Current is not NumberToken numberToken)
			return ErrorBuilder.UnexpectedToken();

		Next();

		expr = new NumberExpression(numberToken);
		return expr;
	}
}