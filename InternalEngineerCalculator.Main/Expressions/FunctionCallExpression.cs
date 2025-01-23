using System.Collections.Immutable;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class FunctionCallExpression(string name, ImmutableArray<Expression> arguments) : Expression
{
	public string Name { get; } = name;

	public ImmutableArray<Expression> Arguments { get; } = arguments;

	public int CountOfArgs => Arguments.Length;
}