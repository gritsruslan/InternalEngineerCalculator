namespace InternalEngineerCalculator.Main.Expressions;

public abstract class AssignmentExpression : Expression
{
	public abstract Expression Expression { get; }

	public abstract string Name { get; }
}