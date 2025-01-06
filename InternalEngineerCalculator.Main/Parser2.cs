using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main;

internal sealed class Parser2(ImmutableArray<Token> tokens)
{
	private readonly ImmutableArray<Token> _tokens = tokens;

	private readonly Token _emptyToken = new NonValueToken(TokenType.EndOfLine, string.Empty);

	private int _position;

	private Token Current => _position < _tokens.Length ? _tokens[_position] : _emptyToken;

	private Token NextToken => _position + 1 < _tokens.Length ? _tokens[_position + 1] : _emptyToken;

	private void Next() => _position++;

	private void Next(int offset) => _position += offset;

	public Result<Expression> Parse()
	{
		var expression = ParseExpression();

		if (Current.Type != TokenType.EndOfLine)
			return new Error("Incorrect end of expression!");

		return expression;
	}

	private Result<Expression> ParseExpression()
	{
		var leftResult = ParseTerm();
		if (!leftResult.TryGetValue(out var left))
			return leftResult;

		while (Current.Type is TokenType.Plus or TokenType.Minus)
		{
			var operationToken = Current as NonValueToken;
			Next();

			var rightResult = ParseTerm();
			if (!rightResult.TryGetValue(out var right))
				return rightResult;

			left = new BinaryExpression(left, operationToken!, right);
		}

		return left;
	}

	private Result<Expression> ParseTerm()
	{
		var leftResult = ParseFactorial();
		if (!leftResult.TryGetValue(out var left))
			return leftResult;

		while (Current.Type is TokenType.Divide or TokenType.Multiply or TokenType.Remainder)
		{
			var operationToken = Current as NonValueToken;
			Next();

			var rightResult = ParseFactorial();
			if (!rightResult.TryGetValue(out var right))
				return rightResult;


			left = new BinaryExpression(left, operationToken!, right);
		}

		return left;
	}

	private Result<Expression> ParseFactorial()
	{
		var expResult = ParseParenthesisExpression();
		if (!expResult.TryGetValue(out var expression))
			return expResult;

		while (Current.Type == TokenType.Factorial)
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

		if (Current.Type == TokenType.Minus)
		{
			Next();
			var inUnaryExpressionResult = ParseExpression();
			if (!inUnaryExpressionResult.TryGetValue(out var inUnaryExpression))
				return inUnaryExpressionResult;
			return new UnaryExpression(inUnaryExpression, UnaryExpressionType.Minus);
		}

		if (Current.Type == TokenType.ModulePipe)
		{
			Next();
			var inModuleExpressionResult = ParseExpression();
			if (!inModuleExpressionResult.TryGetValue(out var inModuleExpression))
				return inModuleExpressionResult;

			if (Current.Type != TokenType.ModulePipe)
				return new Error("Pipe!");
			Next();

			return new UnaryExpression(inModuleExpression, UnaryExpressionType.Module);
		}

		if (Current.Type == TokenType.OpenParenthesis)
		{
			Next();
			var inParenthesisExpressionResult = ParseExpression();
			if (!inParenthesisExpressionResult.TryGetValue(out var inParenthesisExpression))
				return inParenthesisExpressionResult;

			if (Current.Type != TokenType.CloseParenthesis)
				return new Error("Close parenthesis!");
			Next();

			return inParenthesisExpression;
		}

		return new Error("Error in inout!");
	}

	private Result<Expression> ParseFunctionCallExpression()
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
			var parseResult = ParseExpression();
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
				return ErrorBuilder.EndOfInput();
		}

		Next(); // skip close parenthesis
		return new FunctionCallExpression(functionName, [..expressions]);
	}
}