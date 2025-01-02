using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

internal sealed class CommandLineTool
{
	// for future
	private FunctionManager _functionManager;

	private VariableManager _variableManager;

	private Dictionary<string, bool> _environmentVariables;

	public CommandLineTool(FunctionManager functionManager, VariableManager variableManager, Dictionary<string, bool>  environmentVariables)
	{
		_functionManager = functionManager;
		_variableManager = variableManager;
		_environmentVariables = environmentVariables;
	}

	public void ProcessCommand(string command)
	{
		var commandComponents = command.Split(' ');
		var commandName = commandComponents[0].ToLower();
		var args = commandComponents[1..];

		switch (commandName)
		{
			case "#help":
				HelpCommand(args); break;
			case "#exit":
				ExitCommand(args); break;
			case "#clear":
				ClearConsoleCommand(args); break;
			case "#showtokens":
				ShowTokensCommand(args); break;
			case "#showexpressiontree" :
				ShowExpressionTreeCommand(args); break;
			case "#showbasicfunctions":
				ShowBasicFunctions(args); break;
			case "#showvariables":
				ShowVariables(args); break;
			default:
				Console.WriteLine($"Unknown command \"{commandName}\"");
				break;
		}
	}

	private void HelpCommand(string[] args)
	{
		if (args.Length != 0)
		{
			PrintIfIncorrectCountOfArguments("help", 0, args.Length);
			return;
		}

		string helpString =
			"""

			InternalEngineerCalculator by @gritsruslan!

			Available math operators : + - * / ^
			Examples:
			12 + 3 * (2 - 1)
			2 ^ 3 + 52
			1/20 + 1

			Available commands :
			#exit - exit calculator
			#clear - clear console output
			#help - output short calculator guide
			#showexpressiontree - enable showing expression trees
			#unshowexpressiontree - disable showing expression trees

			""";

		Console.WriteLine(helpString);
	}

	private void ExitCommand(string[] args)
	{
		if (args.Length != 0)
		{
			PrintIfIncorrectCountOfArguments("exit", 0, args.Length);
			return;
		}

		Environment.Exit(0);
	}

	private void ClearConsoleCommand(string[] args)
	{
		if (args.Length != 0)
		{
			PrintIfIncorrectCountOfArguments("clear", 0, args.Length);
			return;
		}

		Console.Clear();
	}

	private void ShowTokensCommand(string[] args)
	{
		if (args.Length != 1)
		{
			PrintIfIncorrectCountOfArguments("ShowTokens", 1, args.Length);
			return;
		}

		var arg = args[0].ToLower();

		if (arg == "true")
			_environmentVariables["ShowTokens"] = true;
		else if (arg == "false")
			_environmentVariables["ShowTokens"] = false;
		else
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"Incorrect argument {arg}, expected true or false");
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}

	private void ShowExpressionTreeCommand(string[] args)
	{
		if (args.Length != 1)
		{
			PrintIfIncorrectCountOfArguments("ShowExpressionTree", 1, args.Length);
			return;
		}

		var arg = args[0].ToLower();

		if (arg == "true")
			_environmentVariables["ShowExpressionTree"] = true;
		else if (arg == "false")
			_environmentVariables["ShowExpressionTree"] = false;
		else
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"Incorrect argument {arg}, expected true or false");
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}

	private void ShowBasicFunctions(string[] args)
	{
		if (args.Length != 0)
		{
			PrintIfIncorrectCountOfArguments("ShowBasicFunctions", 0, args.Length);
			return;
		}

		var basicFunctions =
			"""

			Basic functions:

			sin(x)       => Returns the sine of the angle `x` (in radians).
			cos(x)       => Returns the cosine of the angle `x` (in radians).
			tg(x)        => Returns the tangent of the angle `x` (in radians).
			ctg(x)       => Returns the cotangent of the angle `x` (in radians), which is `1 / tan(x)`.
			rad(x)       => Converts an angle `x` from degrees to radians.
			deg(x)       => Converts an angle `x` from radians to degrees.
			sqrt(x)      => Returns the square root of `x`.
			sqrt(x, b)   => Returns the `b`-th root of `x`. Equivalent to `x^(1/b)`.
			floor(x)     => Rounds `x` down to the nearest integer.
			ceil(x)      => Rounds `x` up to the nearest integer.
			round(x)     => Rounds `x` to the nearest integer (rounds half values up).
			log10(x)     => Returns the base-10 logarithm of `x`.
			log(x, b)    => Returns the logarithm of `x` with base `b`.
			ln(x)        => Returns the natural logarithm of `x` (logarithm to the base `e`).
			e(x)         => Returns `e^x`, where `e` is Euler's number (~2.718).
			pow(x, y)    => Returns `x` raised to the power of `y` (`x^y`).

			""";

		Console.WriteLine(basicFunctions);
	}

	private void PrintIfIncorrectCountOfArguments(string command,int mustHaveArgs, int countOfArgs)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		var argsString = mustHaveArgs == 1 ? "arguments" : "argument";
		var transmised = countOfArgs == 1 ? "was" : "were";
		Console.WriteLine(
			$"{command} command must have {mustHaveArgs} {argsString}, but {countOfArgs} {transmised} transmised");
		Console.ForegroundColor = ConsoleColor.Gray;
	}

	private void ShowVariables(string[] args)
	{
		if (args.Length != 0)
		{
			PrintIfIncorrectCountOfArguments("ShowVariables", 0, args.Length);
			return;
		}

		var variables = _variableManager.GetVariables();

		Console.WriteLine("Variables");

		foreach (var (name,value) in variables)
			Console.WriteLine($"{name} : {value}");

		Console.WriteLine();
	}

}