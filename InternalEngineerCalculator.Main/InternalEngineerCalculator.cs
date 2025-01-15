using System.Collections.Immutable;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Extensions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Tokens;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Main;

internal sealed class InternalEngineerCalculator
{
	private const ConsoleColor DefaultColor = ConsoleColor.Gray;

	private readonly FunctionManager _functionManager;

	private readonly VariableManager _variableManager;

	private readonly Evaluator _evaluator;

	private readonly AssignmentExpressionHandler _assignmentExpressionHandler;

	private readonly CommandLineTool _commandLineTool;

	private readonly bool _isDebug;


	private readonly Dictionary<string, bool> _environmentVariables = new()
	{
		{ "ShowTokens", false },
		{ "ShowExpressionTree", false },
	};

	public InternalEngineerCalculator(bool isDebug)
	{
		_isDebug = isDebug;

		_functionManager = new FunctionManager();
		_functionManager.InitializeDefaultFunctions();

		_variableManager = new VariableManager();
		_variableManager.InitializeBasicVariables();

		_evaluator = new Evaluator(_functionManager, _variableManager);

		_assignmentExpressionHandler = new AssignmentExpressionHandler(_evaluator, _variableManager, _functionManager);

		_commandLineTool = new(_functionManager, _variableManager, _environmentVariables);
	}

	public void Start()
	{
		var debugString = _isDebug ? $" : Debug Mode" : string.Empty;
		Console.WriteLine($"InternalEngineerCalculator by @gritsruslan!{debugString}");
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

				var tokensResult = new Lexer(input).Tokenize();
				if (!tokensResult.TryGetValue(out var tokensCollection))
				{
					PrintError(tokensResult.Error);
					continue;
				}

				var tokens = tokensCollection.ToImmutableArray();

				if (_environmentVariables["ShowTokens"])
					tokens.PrintTokens();

				if (Parser.IsAssignmentExpression(tokens))
					HandleAssignmentExpression(tokens);
				else
					HandleResultExpression(tokens);
			}
			catch (Exception e)
			{
				PrintException(e);
			}
		}
	}

	private void HandleAssignmentExpression(ImmutableArray<Token> tokens)
	{
		var parser = new Parser(tokens);

		var assignmentExpressionResult = parser.ParseAssignmentExpression();

		if (!assignmentExpressionResult.TryGetValue(out var assignmentExpression))
		{
			PrintError(assignmentExpressionResult.Error);
			return;
		}

		if(_environmentVariables["ShowExpressionTree"])
			assignmentExpression.PrettyPrint();

		if (assignmentExpression is VariableAssignmentExpression ve)
		{
			var variableAssignmentResult = _assignmentExpressionHandler
				.HandleVariableAssignmentExpression(ve);
			if (!variableAssignmentResult.TryGetValue(out var newVarValue))
			{
				PrintError(variableAssignmentResult.Error);
				return;
			}

			Console.WriteLine(
				$"Variable \"{assignmentExpression.Name}\" received a new value {newVarValue} !");
		}
		else if (assignmentExpression is FunctionAssignmentExpression fe)
		{
			var functionAssignmentResult = _assignmentExpressionHandler.HandleFunctionAssignmentExpression(fe);
			if (functionAssignmentResult.IsFailure)
			{
				PrintError(functionAssignmentResult.Error);
				return;
			}

			Console.WriteLine($"Function \"{fe.Name}\" with {fe.Args.Length} needed arguments was successfully declared!");
		}
	}

	private void HandleResultExpression(ImmutableArray<Token> tokens)
	{
		var parser = new Parser(tokens);

		var expressionResult = parser.Parse();
		if (!expressionResult.TryGetValue(out var expression))
		{
			PrintError(expressionResult.Error);
			return;
		}

		if(_environmentVariables["ShowExpressionTree"])
			expression.PrettyPrint();

		var evaluator = new Evaluator(_functionManager, _variableManager);
		var result = evaluator.Evaluate(expression);
		if (!result.TryGetValue(out var resultValue))
		{
			PrintError(result.Error);
			return;
		}

		if (double.IsNaN(resultValue))
		{
			PrintError(new Error("The result cannot be calculated because its an imaginary number!"));
			return;
		}

		Console.WriteLine($"Result : {resultValue}");
	}

	private void PrintException(Exception exception)
	{
		if (_isDebug)
		{
			Console.WriteLine("Debug error : ");
			Console.WriteLine(exception);
		}
		else
		{
			Console.ForegroundColor = ConsoleColor.DarkRed;
			Console.WriteLine("An unhandled error occurred while the program was running. Please contact @gritsruslan!");
			Console.WriteLine();
			Console.ForegroundColor = DefaultColor;
		}
	}

	private void PrintError(Error error)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine(error.Message);
		Console.WriteLine();
		Console.ForegroundColor = DefaultColor;
	}
}