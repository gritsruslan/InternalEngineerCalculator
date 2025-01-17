using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Expressions;

namespace InternalEngineerCalculator.Main.Functions;

public sealed class FunctionManager
{
	private readonly Dictionary<FunctionInfo, Function> _functions;

	public FunctionManager()
	{
		_functions = new Dictionary<FunctionInfo, Function>();
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
		//SqrtN
		CreateNewBaseFunction("sqrt", 2, args => RMath.Sqrt(args[0], args[1]));
		//Floor
		CreateNewBaseFunction("floor", 1, args => Math.Floor(args[0]));
		//Ceil
		CreateNewBaseFunction("ceil", 1, args => Math.Ceiling(args[0]));
		//Round
		CreateNewBaseFunction("round", 1, args => Math.Round(args[0]));
		//Rad
		CreateNewBaseFunction("rad", 1, args => RMath.DegreeToRadians(args[0]));
		//Deg
		CreateNewBaseFunction("deg", 1, args => RMath.RadiansToDegree(args[0]));
		//Log10
		CreateNewBaseFunction("log10", 1, args => Math.Log10(args[0]));
		//Log
		CreateNewBaseFunction("log", 2, args => Math.Log(args[0], args[1]));
		//Ln
		CreateNewBaseFunction("ln", 1, args => RMath.Ln(args[0]));
		//E
		CreateNewBaseFunction("exp", 1, args => Math.Exp(args[0]));
		//Pow
		CreateNewBaseFunction("pow", 2, args => Math.Pow(args[0], args[1]));
	}

	public bool HasFunction(FunctionInfo header) => _functions.ContainsKey(header);

	public Result<Function> GetFunction(FunctionInfo info)
	{
		if(_functions.TryGetValue(info, out var function))
			return function;

		return ErrorBuilder.FunctionNotFound(info.Name, info.CountOfArg);
	}

	public EmptyResult DeleteFunction(FunctionInfo info)
	{
		var functionResult = GetFunction(info);
		if (!functionResult.TryGetValue(out var function))
			return ErrorBuilder.FunctionNotFound(info.Name, info.CountOfArg);

		if (function.IsBaseFunction)
			return new Error("Cannot delete base function!");

		_functions.Remove(info);

		return EmptyResult.Success();
	}

	public bool CreateNewCustomFunction(string name, IReadOnlyList<string> args, Expression functionExpression)
	{
		var header = new FunctionInfo(name, args.Count);
		var isOverriding = HasFunction(header);
		var convArgs = args.Select(arg => new FunctionArgument(arg));
		var function = new CustomFunction(name, [..convArgs], functionExpression);
		_functions[header] = function;

		return isOverriding;
	}

	private void CreateNewBaseFunction(string name, int countOfArgs, Func<ImmutableArray<double>, double> function)
	{
		_functions.Add(new FunctionInfo(name, countOfArgs),
			new BaseFunction(name, countOfArgs, function));
	}
}