using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Expressions;

internal abstract class Expression
{
	public abstract TokenType Type { get; }
}