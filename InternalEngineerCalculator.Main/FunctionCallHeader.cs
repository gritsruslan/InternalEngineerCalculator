using InternalEngineerCalculator.Main.Exceptions;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main;

public record struct FunctionCallHeader(string FunctionName, int CountOfArg);

internal record FunctionArgument(string Name);

internal abstract class Function(string name)
{
	public readonly string Name = name;

	public abstract bool IsBaseFunction { get; }

	public abstract int CountOfArgs { get; }

	public abstract double Execute(double[] args);
}

internal class FunctionManager
{
	private Dictionary<FunctionCallHeader, Function> _functions;

	public FunctionManager()
	{
		_functions = new Dictionary<FunctionCallHeader, Function>();
	}

	public void InitializeDefaultFunctions()
	{
		// Sin
		CreateNewBaseFunction("sin", 1, args => Math.Sin(args[0]));
		//Cos
		CreateNewBaseFunction("cos", 1, args => Math.Cos(args[0]));
		//Tg
		CreateNewBaseFunction("tg", 1, args => Math.Tan(args[0]));
		//Ctg
		CreateNewBaseFunction("ctg", 1, args => 1/Math.Tan(args[0]));
		//Sqrt2
		CreateNewBaseFunction("sqrt", 1, args => Math.Sqrt(args[0]));
		//SqrtN TODO
		//Floor
		CreateNewBaseFunction("floor", 1, args => Math.Floor(args[0]));
		//Ceil
		CreateNewBaseFunction("ceil", 1, args => Math.Ceiling(args[0]));
		//Round
		CreateNewBaseFunction("round", 1, args => Math.Round(args[0]));
		//Fact TODO
		//Log10
		CreateNewBaseFunction("log10", 1, args => Math.Log10(args[0]));
		//Log
		CreateNewBaseFunction("log", 2, args => Math.Log(args[0], args[1]));
		//Ln
		CreateNewBaseFunction("ln", 1, args => Math.Log(args[0], Math.E));
		//E
		CreateNewBaseFunction("e", 1, args => Math.Exp(args[0]));
		//Pow
		CreateNewBaseFunction("pow", 2, args => Math.Pow(args[0], args[1]));
	}

	public Function GetFunctionByHeader(FunctionCallHeader header)
	{
		if(_functions.TryGetValue(header, out var function))
			return function;

		throw new FunctionNotFoundException(header.FunctionName, header.CountOfArg);
	}

	public void DeleteFunction(string name, int countOfArgs)
	{
		var header = new FunctionCallHeader(name, countOfArgs);

		if(!_functions.Remove(header))
			throw new FunctionNotFoundException(name, countOfArgs);
	}

	private void CreateNewBaseFunction(string name, int countOfArgs, Func<double[], double> function)
	{
		_functions.Add(new FunctionCallHeader(name, countOfArgs),
			new BaseFunction(name, countOfArgs, function));
	}
}

internal sealed class BaseFunction(string name, int countOfArgs, Func<double[], double> function) :
	Function(name)
{
	private readonly Func<double[], double> _function = function;

	public override int CountOfArgs => countOfArgs;

	public override bool IsBaseFunction => true;

	public override double Execute(double[] args) => _function.Invoke(args);
}

internal sealed class FunctionCallExpression(string name, Expression[] arguments) : Expression
{
	public readonly string Name = name;

	public readonly Expression[] Arguments = arguments;

	public override TokenType Type => TokenType.FunctionCall;

	public int CountOfArgs => Arguments.Length;
}

// Future version!
internal sealed class CustomFunction(string name, FunctionArgument[] arguments, Expression functionExpression);
