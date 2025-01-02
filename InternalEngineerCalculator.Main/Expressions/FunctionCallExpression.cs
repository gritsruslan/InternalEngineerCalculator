using System.Collections.Immutable;

namespace InternalEngineerCalculator.Main.Expressions;

internal sealed class FunctionCallExpression(string name, ImmutableArray<Expression> arguments) : Expression
{
	public string Name => name;

	public ImmutableArray<Expression> Arguments => arguments;

	public int CountOfArgs => Arguments.Length;
}