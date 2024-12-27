namespace InternalEngineerCalculator.Main.Expressions;

internal abstract class AssignmentExpression : Expression
{
	public abstract string Name { get; }
}