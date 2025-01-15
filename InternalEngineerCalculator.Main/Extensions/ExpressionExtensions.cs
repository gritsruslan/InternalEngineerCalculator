using InternalEngineerCalculator.Main.Expressions;

namespace InternalEngineerCalculator.Main.Extensions;

internal static class ExpressionExtensions
{
	public static void PrettyPrint(this Expression expression, string offset = "")
	{
		switch (expression)
		{
			case NumberExpression numberExpression:
				NumberExpressionPrint(numberExpression, offset);
				break;
			case UnaryExpression unaryExpression:
				UnaryExpressionPrint(unaryExpression, ref offset);
				break;
			case BinaryExpression binaryExpression:
				BinaryExpressionPrint(binaryExpression, ref offset);
				break;
			case FunctionCallExpression functionCallExpression:
				FunctionCallExpressionPrint(functionCallExpression, ref offset);
				break;
			case VariableExpression variableExpression:
				VariableExpressionPrint(variableExpression, ref offset);
				break;
			case VariableAssignmentExpression variableAssignmentExpression:
				VariableAssignmentExpressionPrint(variableAssignmentExpression, ref offset);
				break;
			case FunctionAssignmentExpression functionAssignmentExpression:
				FunctionAssignmentExpressionPrint(functionAssignmentExpression, ref offset);
				break;
			default:
				throw new Exception("Undefined expression type to print!");
		}
	}

	private static void NumberExpressionPrint(NumberExpression expression, string offset)
	{
		Console.WriteLine(offset + $"Number : {expression.Token.ValueString}");
	}

	private static void UnaryExpressionPrint(UnaryExpression expression, ref string offset)
	{
		offset += "  ";
		Console.WriteLine(offset + $"Operation {expression.Type}");
		expression.Expression.PrettyPrint(offset);
	}

	private static void BinaryExpressionPrint(BinaryExpression expression, ref string offset)
	{
		Console.WriteLine(offset + "Binary Expression");
		offset += "  ";
		expression.Left.PrettyPrint(offset);
		Console.WriteLine(offset + $"Operation {expression.OperationType}");
		expression.Right.PrettyPrint(offset);
	}

	private static void FunctionCallExpressionPrint(FunctionCallExpression expression, ref string offset)
	{
		Console.WriteLine(offset + $"Function {expression.Name}");
		offset += "  ";
		for (int i = 0; i < expression.Arguments.Length; i++)
		{
			Console.WriteLine(offset + $"Argument {i}");
			expression.Arguments[i].PrettyPrint(offset);
		}
	}

	private static void VariableExpressionPrint(VariableExpression expression, ref string offset)
	{
		Console.WriteLine(offset + $"Variable {expression.Name}");
	}

	private static void VariableAssignmentExpressionPrint(VariableAssignmentExpression expression, ref string offset)
	{
		Console.WriteLine($"Variable \"{expression.Name}\" assignment :");
		offset += "  ";
		expression.Expression.PrettyPrint(offset);
	}

	private static void FunctionAssignmentExpressionPrint(FunctionAssignmentExpression expression, ref string offset)
	{
		Console.WriteLine("Function assignment expression : ");
		offset += "  ";
		Console.WriteLine(offset + $"Function name : {expression.Name}");

		foreach (var arg in expression.Args)
			Console.WriteLine(offset + $"Argument {arg}");

		Console.WriteLine(offset + "Function expression : ");
		expression.Expression.PrettyPrint(offset);
	}
}