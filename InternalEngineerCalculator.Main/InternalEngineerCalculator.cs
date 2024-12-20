using InternalEngineerCalculator.Main.Exceptions;
using InternalEngineerCalculator.Main.Extensions;

namespace InternalEngineerCalculator.Main;

internal class InternalEngineerCalculator
{
	public readonly ConsoleColor DefaultColor = ConsoleColor.Gray;

	private bool _showExpressionTree;

#if DEBUG
	public void StartDebug()
	{
		Console.WriteLine($"InternalEngineerCalculator by @gritsruslan! : Debug Mode");
		while (true)
		{
			Console.Write("> ");
			var input = Console.ReadLine();

			if (string.IsNullOrWhiteSpace(input))
				continue;

			if (input[0] == '#')
			{
				ProcessCommandLine(input);
				continue;
			}

			var tokens = new Lexer(input).Tokenize();
			//tokens.PrintTokens();
			var parser = new Parser(tokens);
			var expression = parser.ParseExpression();

			//expression.PrettyPrint();
			var evaluator = new Evaluator();
			var result = evaluator.Evaluate(expression);

			if(_showExpressionTree)
				expression.PrettyPrint();

			Console.WriteLine($"Result : {result}");
		}
	}
#endif

	public void Start()
	{
		Console.WriteLine($"InternalEngineerCalculator by @gritsruslan!");
		while (true)
		{
			try
			{
				Console.Write("> ");
				var input = Console.ReadLine();

				if (string.IsNullOrWhiteSpace(input))
					continue;

				if (input[0] == '#')
				{
					ProcessCommandLine(input);
					continue;
				}

				var lexemes = new Lexer(input).Tokenize();
				var parser = new Parser(lexemes);
				var expression = parser.ParseExpression();
				var evaluator = new Evaluator();
				var result = evaluator.Evaluate(expression);

				if(_showExpressionTree)
					expression.PrettyPrint();

				Console.WriteLine($"Result : {result}");
			}
			catch (CalculatorException exception)
			{
				Console.ForegroundColor = ConsoleColor.DarkMagenta;
				Console.WriteLine(exception.Message);
				Console.WriteLine();
				Console.ForegroundColor = DefaultColor;
			}
			catch (Exception exception)
			{
				Console.ForegroundColor = ConsoleColor.DarkRed;
				Console.WriteLine("An unhandled error occurred while the program was running. Please contact @gritsruslan!");
				Console.WriteLine();
				Console.ForegroundColor = DefaultColor;
			}
		}
	}

	private void ProcessCommandLine(string commandLine)
	{
		var commandLineNames =
			commandLine.Split(' ', StringSplitOptions.RemoveEmptyEntries).Select(str => str.ToLower()).ToArray();
		var command = commandLineNames[0];

		if(command == "#exit")
			Environment.Exit(0);
		else if(command == "#clear")
			Console.Clear();
		else if (command == "#help")
		{
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
		else if (command == "#showexpressiontree")
		{
			Console.WriteLine("Display expression tree is enabled!");
			_showExpressionTree = true;
		}
		else if (command == "#unshowexpressiontree")
		{
			Console.WriteLine("Display expression tree is disabled!");
			_showExpressionTree = false;
		}
		else
		{
			Console.WriteLine($"Unknown command : {command}");
		}
	}
}