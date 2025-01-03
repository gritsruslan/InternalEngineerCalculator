using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Expressions;

namespace InternalEngineerCalculator.Main.Functions;

internal sealed class FunctionManager
{
	private readonly Dictionary<FunctionCallHeader, Function> _functions;

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
		//SqrtN
		CreateNewBaseFunction("sqrt", 2, args => Math.Exp(Math.Log(args[0], Math.E) / args[1]) );
		//Floor
		CreateNewBaseFunction("floor", 1, args => Math.Floor(args[0]));
		//Ceil
		CreateNewBaseFunction("ceil", 1, args => Math.Ceiling(args[0]));
		//Round
		CreateNewBaseFunction("round", 1, args => Math.Round(args[0]));
		//Rad
		CreateNewBaseFunction("rad", 1, args => args[0] * 180 / Math.PI);
		//Deg
		CreateNewBaseFunction("deg", 1, args => args[0] * Math.PI / 180);
		//Fact
		CreateNewBaseFunction("fact", 1, args =>
		{
			var n = args[0];

			double result = 1;
			for (int i = 2; i <= n; i++)
				result *= i;
			return result;
		});

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

	public Result<Function> GetFunctionByHeader(FunctionCallHeader header)
	{
		if(_functions.TryGetValue(header, out var function))
			return function;

		return ErrorBuilder.FunctionNotFound(header.FunctionName, header.CountOfArg);
	}

	public bool HasFunction(FunctionCallHeader header) => _functions.ContainsKey(header);

	public EmptyResult CreateNewCustomFunction(string name, IReadOnlyList<string> args, Expression functionExpression)
	{
		var header = new FunctionCallHeader(name, args.Count);

		if (HasFunction(header))
			return new Error(
				$"There are a function with name \"{name}\" and {args.Count} arguments. " +
				$"If you want to override it, first of all delete old function!");

		var convArgs = args.Select(arg => new FunctionArgument(arg));

		var function = new CustomFunction(name, [..convArgs], functionExpression);

		_functions.Add(header, function);

		return EmptyResult.Success();
	}

	public EmptyResult DeleteFunction(string name, int countOfArgs)
	{
		var header = new FunctionCallHeader(name, countOfArgs);

		if (!_functions.Remove(header))
			return ErrorBuilder.FunctionNotFound(name, countOfArgs);

		return EmptyResult.Success();
	}

	private void CreateNewBaseFunction(string name, int countOfArgs, Func<ImmutableArray<double>, double> function)
	{
		_functions.Add(new FunctionCallHeader(name, countOfArgs),
			new BaseFunction(name, countOfArgs, function));
	}
}