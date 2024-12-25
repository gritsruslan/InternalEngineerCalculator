using InternalEngineerCalculator.Main.Exceptions;

namespace InternalEngineerCalculator.Main.Functions;

internal class FunctionManager
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
			if (n < 0)
				throw new CalculatorException("The factorial of negative number is not defined!");

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