namespace InternalEngineerCalculator.Main;

internal static class ExpressionExtensions
{
	public static void PrettyPrint(this Expression expression, string offset = "")
	{
		if(expression is NumberExpression numberExpression)
			Console.WriteLine(offset + $"Number : {numberExpression.Token.ValueString}");

		if (expression is UnaryExpression unaryExpression)
		{
			offset += "  ";
			Console.WriteLine(offset + $"Operation {unaryExpression.UnaryOperation.ValueString}");
			unaryExpression.Expression.PrettyPrint(offset);
		}

		if (expression is BinaryExpression binaryExpression)
		{
			Console.WriteLine(offset + "Binary Expression");
			offset += "  ";
			binaryExpression.Left.PrettyPrint(offset);
			Console.WriteLine(offset + $"Operation {binaryExpression.Operation.ValueString}");
			binaryExpression.Right.PrettyPrint(offset);
		}
	}
}