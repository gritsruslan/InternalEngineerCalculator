namespace InternalEngineerCalculator.Main.Expressions;

internal abstract class AssignmentExpression : Expression
{
	public abstract Expression Expression { get; }

	public abstract string Name { get; }
}