using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal class FunctionAssignmentExpression(string name, ImmutableArray<string> args, Expression expression) : AssignmentExpression
{
	private readonly ImmutableArray<string> _args = args;

	public IReadOnlyList<string> Args => _args;

	public override string Name => name;

	public override Expression Expression => expression;

	public override TokenType Type => TokenType.FunctionAssignmentExpression;
}