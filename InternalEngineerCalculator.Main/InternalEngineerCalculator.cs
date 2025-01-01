using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Exceptions;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Extensions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

internal class InternalEngineerCalculator
{
	public readonly ConsoleColor DefaultColor = ConsoleColor.Gray;

	private readonly FunctionManager _functionManager;

	private readonly VariableManager _variableManager;

	private readonly Evaluator _evaluator;

	private readonly AssignmentExpressionHandler _assignmentExpressionHandler;

	private readonly CommandLineTool _commandLineTool;

	private readonly Dictionary<string, bool> _environmentVariables = new()
	{
		{ "ShowTokens", false },
		{ "ShowExpressionTree", false },
	};

	public InternalEngineerCalculator()
	{
		_functionManager = new FunctionManager();
		_functionManager.InitializeDefaultFunctions();

		_variableManager = new VariableManager();
		_variableManager.InitializeBasicVariables();

		_evaluator = new Evaluator(_functionManager, _variableManager);

		_assignmentExpressionHandler = new AssignmentExpressionHandler(_evaluator, _variableManager, _functionManager);

		_commandLineTool = new(_functionManager, _variableManager, _environmentVariables);
	}

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
				_commandLineTool.ProcessCommand(input);
				continue;
			}

			var tokens = new Lexer(input).Tokenize().ToImmutableArray();

			if(_environmentVariables["ShowTokens"])
				tokens.PrintTokens();

			if (Parser.IsAssignmentExpression(tokens))
			{
				var parser = new Parser(tokens);

				var assignmentExpression = parser.ParseAssignmentExpression();

				if(_environmentVariables["ShowExpressionTree"])
					assignmentExpression.PrettyPrint();

				if (assignmentExpression is VariableAssignmentExpression ve)
				{
					var newVarValue = _assignmentExpressionHandler
						.HandleVariableAssignmentExpression(ve);

					Console.WriteLine(
						$"Variable \"{assignmentExpression.Name}\" received a new value {newVarValue} !");
				}
				else if (assignmentExpression is FunctionAssignmentExpression fe)
				{
					_assignmentExpressionHandler.HandleFunctionAssignmentExpression(fe);

					Console.WriteLine($"Function \"{fe.Name}\" with {fe.Args.Count} needed arguments was successfully declared!");
				}
			}
			else
			{
				var parser = new Parser(tokens);
				var expression = parser.ParseExpression();

				if(_environmentVariables["ShowExpressionTree"])
					expression.PrettyPrint();

				var evaluator = new Evaluator(_functionManager, _variableManager);
				var result = evaluator.Evaluate(expression);

				Console.WriteLine($"Result : {result}");
			}
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
					_commandLineTool.ProcessCommand(input);
					continue;
				}

				var tokens = new Lexer(input).Tokenize().ToImmutableArray();

				if(_environmentVariables["ShowTokens"])
					tokens.PrintTokens();

				if (Parser.IsAssignmentExpression(tokens))
				{
					var parser = new Parser(tokens);

					var assignmentExpression = parser.ParseAssignmentExpression();

					if(_environmentVariables["ShowExpressionTree"])
						assignmentExpression.PrettyPrint();

					if (assignmentExpression is VariableAssignmentExpression ve)
					{
						var newVarValue = _assignmentExpressionHandler
							.HandleVariableAssignmentExpression(ve);

						Console.WriteLine(
							$"Variable \"{assignmentExpression.Name}\" received a new value {newVarValue} !");
					}
					else if (assignmentExpression is FunctionAssignmentExpression fe)
					{
						_assignmentExpressionHandler.HandleFunctionAssignmentExpression(fe);

						Console.WriteLine($"Function \"{fe.Name}\" with {fe.Args.Count} needed arguments was successfully declared!");
					}
				}
				else
				{
					var parser = new Parser(tokens);
					var expression = parser.ParseExpression();

					if(_environmentVariables["ShowExpressionTree"])
						expression.PrettyPrint();

					var evaluator = new Evaluator(_functionManager, _variableManager);
					var result = evaluator.Evaluate(expression);

					Console.WriteLine($"Result : {result}");
				}
			}
			catch (CalculatorException exception)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("Input error" + exception.Message);
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
}