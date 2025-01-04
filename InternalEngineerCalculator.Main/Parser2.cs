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

			Next();

			left = new BinaryExpression(left, operationToken!, right);
		}

		return left;
	}

	private Result<Expression> ParseTerm()
	{
		var leftResult = ParseFactorial();
		if (!leftResult.TryGetValue(out var left))
			return leftResult;

		while (Current.Type is TokenType.Divide or TokenType.Multiply)
		{
			var operationToken = Current as NonValueToken;
			Next();

			var rightResult = ParseFactorial();
			if (!rightResult.TryGetValue(out var right))
				return rightResult;

			Next();

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
		Next();

		if (Current.Type == TokenType.Number)
			return new NumberExpression((token as NumberToken)!);

		if (Current.Type == TokenType.Identifier)
			return new VariableExpression(Current);

		if (Current.Type == TokenType.Minus)
		{
			var inUnaryExpressionResult = ParseExpression();
			if (!inUnaryExpressionResult.TryGetValue(out var inUnaryExpression))
				return inUnaryExpressionResult;
			return new UnaryExpression(inUnaryExpression, UnaryExpressionType.Minus);
		}

		if (Current.Type == TokenType.ModulePipe)
		{
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
}