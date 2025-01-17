using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

internal sealed class CommandLineTool(
	Dictionary<FunctionInfo, string> functionAssignmentStrings,
	FunctionManager functionManager,
	VariableManager variableManager,
	Dictionary<string, bool> environmentVariables)
{
	private readonly FunctionManager _functionManager = functionManager;

	private readonly VariableManager _variableManager = variableManager;

	private readonly Dictionary<string, bool> _environmentVariables = environmentVariables;

	private readonly Dictionary<FunctionInfo, string> _functionAssignmentStrings = functionAssignmentStrings;


	public void ProcessCommand(string command)
	{
		var commandComponents = command.Split(' ')
			.Where(s => !string.IsNullOrWhiteSpace(s)).ToImmutableArray();

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
				ShowExpressionTree(args); break;
			case "#showbasicfunctions":
				ShowBasicFunctions(args); break;
			case "#showvariables":
				ShowVariables(args); break;
			case "#deletevariable":
				DeleteVariable(args); break;
			case "#showcustomfunctions":
				ShowCustomFunctions(args); break;
			case "#deletefunction" :
				DeleteCustomFunction(args); break;
			default:
				Console.WriteLine($"Unknown command \"{commandName}\"");
				break;
		}
	}

	private void HelpCommand(ImmutableArray<string> args)
	{
		if (args.Length != 0)
		{
			PrintIfIncorrectCountOfArguments("help", 0, args.Length);
			return;
		}
//TODO
		string helpString =

			"""
			InternalEngineerCalculator by @gritsruslan!

			Range of possible values : +/- 1.7E-308 to 1.7E+308
			Available math operators : + - * / ^ % !

			Examples:
			12 + 3 * (2 - 1)
			2 ^ 3 + 52
			1/20 + 1
			-3! + 2

			Available commands :
			#exit - exit calculator
			#clear - clear console output
			#help - output short calculator guide
			#ShowTokens <true | false> - enable or disable tokens
			#ShowExpressionTree <true | false> - enable or disable showing expression trees
			#ShowBasicFunctions - show basic calculator functions
			#ShowCustomFunctions - shows user defined functions
			#DeleteFunction <name> <countOfArgs> - delete user defined function
			#ShowVariables - show user defined variables
			#DeleteVariable <name> - delete variable
			""";

		Console.WriteLine(helpString);
	}

	private void ExitCommand(ImmutableArray<string> args)
	{
		if (args.Length != 0)
		{
			PrintIfIncorrectCountOfArguments("exit", 0, args.Length);
			return;
		}

		Environment.Exit(0);
	}

	private void ClearConsoleCommand(ImmutableArray<string> args)
	{
		if (args.Length != 0)
		{
			PrintIfIncorrectCountOfArguments("clear", 0, args.Length);
			return;
		}

		Console.Clear();
	}

	private void ShowTokensCommand(ImmutableArray<string> args)
	{
		if (args.Length != 1)
		{
			PrintIfIncorrectCountOfArguments("ShowTokens", 1, args.Length);
			return;
		}

		var arg = args[0].ToLower();

		if (arg == "true")
		{
			_environmentVariables["ShowTokens"] = true;
			Console.WriteLine("Token display enabled!");
		}
		else if (arg == "false")
		{
			_environmentVariables["ShowTokens"] = false;
			Console.WriteLine("Token display disabled!");
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"Incorrect argument {arg}, expected true or false");
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}

	private void ShowExpressionTree(ImmutableArray<string> args)
	{
		if (args.Length != 1)
		{
			PrintIfIncorrectCountOfArguments("ShowExpressionTree", 1, args.Length);
			return;
		}

		var arg = args[0].ToLower();

		if (arg == "true")
		{
			_environmentVariables["ShowExpressionTree"] = true;
			Console.WriteLine("Display expression tree enabled.");
		}
		else if (arg == "false")
		{
			_environmentVariables["ShowExpressionTree"] = false;
			Console.WriteLine("Display expression tree disabled.");
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine($"Incorrect argument {arg}, expected true or false");
			Console.ForegroundColor = ConsoleColor.Gray;
		}
	}

	private void ShowBasicFunctions(ImmutableArray<string> args)
	{
		if (args.Length != 0)
		{
			PrintIfIncorrectCountOfArguments("ShowBasicFunctions", 0, args.Length);
			return;
		}

		var basicFunctions =
			"""
			=========================================
			Basic functions:

			sin(x)       => Returns the sine of the angle `x` (in radians).
			cos(x)       => Returns the cosine of the angle `x` (in radians).
			tg(x)        => Returns the tangent of the angle `x` (in radians).
			ctg(x)       => Returns the cotangent of the angle `x` (in radians), which is `1 / tan(x)`.
			sqrt(x)      => Returns the square root of `x`.
			sqrt(x, b)   => Returns the `b`-th root of `x`. Equivalent to `x^(1/b)`.
			floor(x)     => Rounds `x` down to the nearest integer.
			ceil(x)      => Rounds `x` up to the nearest integer.
			round(x)     => Rounds `x` to the nearest integer (rounds half values up).
			rad(x)       => Converts an angle `x` from degrees to radians.
			deg(x)       => Converts an angle `x` from radians to degrees.
			log10(x)     => Returns the base-10 logarithm of `x`.
			log(x, b)    => Returns the logarithm of `x` with base `b`.
			ln(x)        => Returns the natural logarithm of `x` (logarithm to the base `e`).
			exp(x)         => Returns `e^x`, where `e` is Euler's number (~2.718).
			pow(x, y)    => Returns `x` raised to the power of `y` (`x^y`).
			=========================================
			""";

		Console.WriteLine(basicFunctions);
	}

	private void PrintIfIncorrectCountOfArguments(string command, int mustHaveArgs, int countOfArgs)
	{
		var argsString = mustHaveArgs == 1 ? "arguments" : "argument";
		var transmised = countOfArgs == 1 ? "was" : "were";
		PrintError(
			$"{command} command must have {mustHaveArgs} {argsString}, but {countOfArgs} {transmised} transmised");
	}

	private void PrintError(string message)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine(message);
		Console.ForegroundColor = ConsoleColor.Gray;

	}

	private void ShowCustomFunctions(ImmutableArray<string> args)
	{
		if (args.Length != 0)
		{
			PrintIfIncorrectCountOfArguments("ShowCustomFunctions", 0, args.Length);
			return;
		}

		Console.WriteLine("=========================================");
		Console.WriteLine("Custom Functions : ");

		foreach (var function in _functionAssignmentStrings)
			Console.WriteLine(function.Value);

		Console.WriteLine("=========================================");
	}

	private void DeleteCustomFunction(ImmutableArray<string> args)
	{
		if (args.Length != 2)
		{
			PrintIfIncorrectCountOfArguments("DeleteFunction", 2, args.Length);
			return;
		}

		var name = args[0];
		if (!int.TryParse(args[1], out var countOfArgs))
		{
			PrintError($"The \"{args[1]}\" is not correct argument!");
			return;
		}

		var info = new FunctionInfo(name, countOfArgs);
		var deleteResult = _functionManager.DeleteFunction(info);

		if(deleteResult.IsFailure)
			PrintError(deleteResult.Error.Message);
		else
		{
			_functionAssignmentStrings.Remove(info);
			Console.WriteLine($"Function \"{name}\" with \"{countOfArgs}\" was successfully deleted!");
		}
	}

	private void DeleteVariable(ImmutableArray<string> args)
	{
		if (args.Length != 1)
		{
			PrintIfIncorrectCountOfArguments("DeleteVariable", 1, args.Length);
			return;
		}

		var deleteResult = _variableManager.DeleteVariable(args[0]);

		if(deleteResult.IsSuccess)
			Console.WriteLine($"Variable \"{args[0]}\" was successfully deleted!");
		else
			PrintError(deleteResult.Error.Message);
	}

	private void ShowVariables(ImmutableArray<string> args)
	{
		if (args.Length != 0)
		{
			PrintIfIncorrectCountOfArguments("ShowVariables", 0, args.Length);
			return;
		}

		var variables = _variableManager.GetVariables();

		Console.WriteLine("=========================================");
		Console.WriteLine("Variables : ");
		foreach (var (name,value) in variables)
			Console.WriteLine($"{name} = {value.Value}");
		Console.WriteLine("=========================================");
	}
}