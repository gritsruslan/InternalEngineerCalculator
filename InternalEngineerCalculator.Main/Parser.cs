using System.Collections.Immutable;
using System.Diagnostics;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main;

/// <summary> Parse collection of tokens into AST tree expression </summary>
public sealed class Parser(ImmutableArray<Token> tokens)
{
	private readonly Token _emptyToken = new NonValueToken(TokenType.EndOfLine, string.Empty);

	private int _position;

	private Token Current => _position < tokens.Length ? tokens[_position] : _emptyToken;

	private Token NextToken => _position + 1 < tokens.Length ? tokens[_position + 1] : _emptyToken;

	private void Next() => _position++;

	private void Next(int offset) => _position += offset;

	public static bool IsAssignmentExpression(IEnumerable<Token> tokens) =>
		tokens.Any(t => t.Type == TokenType.EqualSign);

	private bool IsVariableAssignmentExpression() => Current.Type == TokenType.Identifier;

	private bool IsFunctionAssignmentExpression() =>
		Current.Type == TokenType.Identifier && NextToken.Type == TokenType.OpenParenthesis;

	public Result<Expression> Parse()
	{
		var expression = ParseWithPrecedence();

		if (Current.Type != TokenType.EndOfLine)
			return ErrorBuilder.IncorrectExpression();

		return expression;
	}

	#region Assignment
	public Result<AssignmentExpression> ParseAssignmentExpression()
	{

		if (IsFunctionAssignmentExpression())
			return ParseFunctionAssignmentExpression();

		if (IsVariableAssignmentExpression())
			return ParseVariableAssignmentExpression();

		return new Error("Incorrect assignment expression!");
	}

	private Result<AssignmentExpression> ParseVariableAssignmentExpression()
	{
		var variableIdentifierToken = Current;

		Next();

		if (Current.Type != TokenType.EqualSign)
			return new Error("Incorrect assignment expression!");

		Next();

		var variableExpressionResult = Parse();
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

		var functionExpressionResult = Parse();
		if (!functionExpressionResult.TryGetValue(out var functionExpression))
			return functionExpressionResult.Error;

		return new FunctionAssignmentExpression(functionName, [..args], functionExpression);
	}

	private static BinaryOperationType GetBinaryOperationType(NonValueToken token)
	{
		return token.Type switch
		{
			TokenType.Plus => BinaryOperationType.Addition,
			TokenType.Minus => BinaryOperationType.Subtraction,
			TokenType.Divide => BinaryOperationType.Division,
			TokenType.Multiply => BinaryOperationType.Multiplication,
			TokenType.Pow => BinaryOperationType.Power,
			TokenType.Remainder => BinaryOperationType.Remainder,
			_ => throw new ArgumentException("Unknown operation token!")
		};
	}

	#endregion

	/// <summary> parsing expressions taking into account the math order of operations </summary>
	/// The idea is partly taken from https://github.com/terrajobst/minsk
	private Result<Expression> ParseWithPrecedence(int parentPrecedence = 0)
	{
		var leftResult = ParseFactorial();
		if (!leftResult.TryGetValue(out var left))
			return leftResult;

		while (true)
		{
			var operationToken = Current as NonValueToken;

			if (operationToken is null)
				return ErrorBuilder.IncorrectExpression();

			var operationPrecedence = GetOperatorPrecedence(operationToken.Type);

			// if unknown operation token (zero) or precedence is less or equals than parent precedence
			if(operationPrecedence == 0 || operationPrecedence <= parentPrecedence)
				break;

			Next();

			var rightResult = ParseWithPrecedence(operationPrecedence);
			if (!rightResult.TryGetValue(out var right))
				return rightResult;

			left = new BinaryExpression(left, GetBinaryOperationType(operationToken), right);
		}

		return left;
	}

	private static int GetOperatorPrecedence(TokenType type)
	{
		return type switch
		{
			TokenType.Plus => 1,
			TokenType.Minus => 1,
			TokenType.Multiply => 2,
			TokenType.Divide => 2,
			TokenType.Remainder => 2,
			TokenType.Pow => 3,
			_ => 0
		};
	}

	/// <summary> Parsing factorials, like 5!, 3!</summary>
	private Result<Expression> ParseFactorial()
	{
		var expResult = ParseParenthesisExpression();
		if (!expResult.TryGetValue(out var expression))
			return expResult;

		while (Current.Type is TokenType.Factorial)
		{
			Next();
			expression = new UnaryExpression(expression, UnaryExpressionType.Factorial);
		}

		return expression;
	}

	private Result<Expression> ParseParenthesisExpression()
	{
		var token = Current;

		if (Current.Type == TokenType.Number)
		{
			Next();
			return new NumberExpression((token as NumberToken)!);
		}

		// if its function call
		if (Current.Type == TokenType.Identifier && NextToken.Type == TokenType.OpenParenthesis)
		{
			var exprResult =  ParseFunctionCallExpression();
			return exprResult;
		}

		if (Current.Type == TokenType.Identifier)
		{
			Next();
			return new VariableExpression(token);
		}

		// if its unary minus operation (like -3, -1)
		if (Current.Type == TokenType.Minus)
		{
			Next();
			var inUnaryExpressionResult = ParseFactorial();
			if (!inUnaryExpressionResult.TryGetValue(out var inUnaryExpression))
				return inUnaryExpressionResult;
			return new UnaryExpression(inUnaryExpression, UnaryExpressionType.Minus);
		}

		if (Current.Type == TokenType.ModulePipe)
		{
			Next();
			var inModuleExpressionResult = ParseWithPrecedence();
			if (!inModuleExpressionResult.TryGetValue(out var inModuleExpression))
				return inModuleExpressionResult;

			if (Current.Type != TokenType.ModulePipe)
				return new Error("Incorrect expression! Module expression must have close pipe!");
			Next();

			return new UnaryExpression(inModuleExpression, UnaryExpressionType.Module);
		}

		// parse parenthesis expression
		if (Current.Type == TokenType.OpenParenthesis)
		{
			Next();
			var inParenthesisExpressionResult = ParseWithPrecedence();
			if (!inParenthesisExpressionResult.TryGetValue(out var inParenthesisExpression))
				return inParenthesisExpressionResult;

			if (Current.Type != TokenType.CloseParenthesis)
				return new Error("Incorrect expression! Parenthesis expression must have close parentesis!");
			Next();

			return inParenthesisExpression;
		}

		Debug.Assert(false, "Error in inout!");
		return ErrorBuilder.IncorrectExpression();
	}

	private Result<Expression> ParseFunctionCallExpression()
	{
		var functionName = Current.ValueString;

		// skip function name token and open parenthesis token
		Next(2);

		if (Current.Type == TokenType.EndOfLine)
			return ErrorBuilder.IncorrectExpression();

		if (Current.Type == TokenType.CloseParenthesis)
			return new Error("Function cannot have zero arguments!");

		var expressions = new List<Expression>();

		// while the function is finished parsing its arguments into expressions
		while (Current.Type != TokenType.CloseParenthesis)
		{
			var parseResult = ParseWithPrecedence();
			if (!parseResult.TryGetValue(out var expression))
				return parseResult.Error;
			expressions.Add(expression);

			if (Current.Type == TokenType.EndOfLine)
				return new Error("Function call must have close parenthesis!");

			if (Current.Type != TokenType.Comma && Current.Type != TokenType.CloseParenthesis)
				return new Error("Expected close parenthesis or comma in function call!");

			if (Current.Type == TokenType.Comma)
				Next(); // skip comma

			if (Current.Type == TokenType.EndOfLine)
				return ErrorBuilder.IncorrectExpression();
		}

		Next(); // skip close parenthesis
		return new FunctionCallExpression(functionName, [..expressions]);
	}
}