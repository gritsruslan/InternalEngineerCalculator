using InternalEngineerCalculator.Main.Exceptions;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main;

internal sealed class Evaluator
{
	public double Evaluate(Expression expression)
	{
		if (expression is NumberExpression ne)
			return ne.Token.Value;

		if (expression is UnaryExpression ue)
		{
			var expr = Evaluate(ue.Expression);

			if (ue.UnaryOperation.Type == TokenType.Minus)
				expr *= -1;
			// Other unary operators
			return expr;
		}

		var binExpression = expression as BinaryExpression;

		var left = Evaluate(binExpression!.Left);
		var operation = binExpression.Operation;
		var right = Evaluate(binExpression.Right);

		double result = operation.Type switch
		{
			TokenType.Plus => left + right,
			TokenType.Minus => left - right,
			TokenType.Multiply => left * right,
			TokenType.Divide => right == 0 ? throw new CalculatorDivideByZeroException() : left/right,
			TokenType.Pow => Math.Pow(left, right),
			_ => throw new CalculatorException("Unknown math operator!")
		};

		return result;
	}
}