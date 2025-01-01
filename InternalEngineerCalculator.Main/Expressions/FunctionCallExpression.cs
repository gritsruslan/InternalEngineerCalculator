using System.Collections.Immutable;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class FunctionCallExpression(string name, ImmutableArray<Expression> arguments) : Expression
{
	public readonly string Name = name;

	public readonly ImmutableArray<Expression> Arguments = arguments;

	public int CountOfArgs => Arguments.Length;
}