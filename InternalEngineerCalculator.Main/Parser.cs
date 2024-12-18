using InternalEngineerCalculator.Main.Exceptions;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main;

internal sealed class Parser
{
	private List<Token> _tokens;

	private int _position;

	private Token? Current => _position < _tokens.Count ? _tokens[_position] : null;

	private void Next() => _position++;
	public Parser(List<Token> tokens)
	{
		_tokens = tokens;
	}

	public Expression ParseExpression(int parentPrecedence = 0)
	{
		Expression left;
		if (Current is null)
			throw new EndOfInputException();

		var unaryOperatorPrecedence = GetUnaryOperatorPrecedence(Current);
		if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
		{
			var operatorToken = Current as NonValueToken;
			Next();
			var operand = ParseExpression(unaryOperatorPrecedence);
			left = new UnaryExpression(operatorToken!, operand);
		}
		else
		{
			left = ParseParenthesis();
		}

		while (true)
		{
			if(Current is null)
				break;

			if (Current is not NonValueToken operatorToken)
				throw new UnexpectedTokenException(Current.Type.ToString(), "math operator");

			var precedence = GetOperationPrecedence(operatorToken);

			if (precedence == 0 || precedence <= parentPrecedence)
				break;

			Next();
			var right = ParseExpression(precedence);
			left = new BinaryExpression(left, operatorToken, right);
		}

		return left;
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
		if (Current.Type == TokenType.OpenParenthesis)
		{
			Next();
			expr = ParseExpression();

			if (Current == null)
				throw new EndOfInputException();

			if (Current.Type != TokenType.CloseParenthesis)
				throw new UnexpectedTokenException(Current.Type,TokenType.CloseParenthesis );

			Next();
			return expr;
		}

		if (Current is not NumberToken numberToken)
			throw new UnexpectedTokenException(Current.Type,TokenType.Number);

		Next();
		expr = new NumberExpression(numberToken);
		return expr;
	}
}