using InternalEngineerCalculator.Main.Expressions;

namespace InternalEngineerCalculator.Main.Extensions;

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

		if (expression is FunctionCallExpression functionExpression)
		{
			Console.WriteLine(offset + $"Function {functionExpression.Name}");
			offset += "  ";
			for (int i = 0; i < functionExpression.Arguments.Length; i++)
			{
				Console.WriteLine(offset + $"Argument {i}");
				functionExpression.Arguments[i].PrettyPrint(offset);
			}
		}

		if (expression is VariableExpression variableExpression)
			Console.WriteLine(offset + $"Variable {variableExpression.Name}");

		if (expression is VariableAssignmentExpression variableAssignmentExpression)
		{
			Console.WriteLine($"Variable \"{variableAssignmentExpression.VariableName}\" assignment :");
			offset += "  ";
			variableAssignmentExpression.VariableValueExpression.PrettyPrint(offset);
		}
	}
}