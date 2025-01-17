using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
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

	private readonly Logger _logger = new();

	private readonly Lexer _lexer = new();

	private readonly Evaluator _evaluator;

	private readonly AssignmentExpressionHandler _assignmentExpressionHandler;

	private readonly CommandLineTool _commandLineTool;

	private readonly Dictionary<FunctionInfo, string> _functionAssignmentStrings = [];

	private readonly Dictionary<string, bool> _environmentVariables = new()
	{
		{ "ShowTokens", false },
		{ "ShowExpressionTree", false },
	};

	public InternalEngineerCalculator()
	{
		var functionManager = new FunctionManager();
		var variableManager = new VariableManager();

		functionManager.InitializeDefaultFunctions();
		variableManager.InitializeBasicVariables();

		_evaluator = new Evaluator(functionManager, variableManager);
		_assignmentExpressionHandler = new AssignmentExpressionHandler(_evaluator, variableManager, functionManager);
		_commandLineTool = new(_functionAssignmentStrings, functionManager, variableManager, _environmentVariables);
	}

	[DoesNotReturn]
	public void StartLoop()
	{
#if DEBUG
		Console.WriteLine("InternalEngineerCalculator by @gritsruslan! : Debug Mode");
#else
		Console.WriteLine("InternalEngineerCalculator by @gritsruslan!");
#endif
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

				var tokensResult = _lexer.Tokenize(input);
				if (!tokensResult.TryGetValue(out var tokensCollection))
				{
					PrintError(tokensResult.Error);
					continue;
				}

				var tokens = tokensCollection.ToImmutableArray();

				if (_environmentVariables["ShowTokens"])
					tokens.PrintTokens();

				if (Parser.IsAssignmentExpression(tokens))
					HandleAssignmentExpression(input, tokens);
				else
					HandleResultExpression(tokens);
			}
			catch (Exception e)
			{
				_logger.LogException(e);
				PrintException(e);
			}
		}
	}

	private void HandleAssignmentExpression(string assignmentString, ImmutableArray<Token> tokens)
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
			var isOverriding = _assignmentExpressionHandler.HandleFunctionAssignmentExpression(fe);
			_functionAssignmentStrings[new FunctionInfo(fe.Name, fe.Args.Length)] = assignmentString;
			Console.WriteLine(isOverriding
				? $"Function \"{fe.Name}\" with {fe.Args.Length} needed arguments was successfully overrided!"
				: $"Function \"{fe.Name}\" with {fe.Args.Length} needed arguments was successfully declared!");
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

		var result = _evaluator.Evaluate(expression);
		if (!result.TryGetValue(out var resultValue))
		{
			PrintError(result.Error);
			return;
		}

		if (double.IsNaN(resultValue))
			PrintError("The result cannot be calculated because its an imaginary number!");
		else if (double.IsSubnormal(resultValue))
			PrintError("The result cannot be calculated because its a subnormal number!");
		else if (double.IsInfinity(resultValue))
			PrintError("The result cannot be calculated because its out of possible value range!");
		else
			Console.WriteLine($"Result : {resultValue}");
	}

	private void PrintException(Exception exception)
	{
#if DEBUG
		Console.WriteLine("Debug error : ");
		Console.WriteLine(exception);
#else
		Console.ForegroundColor = ConsoleColor.DarkRed;
		Console.WriteLine("An unhandled error occurred while the program was running. Please contact @gritsruslan!");
		Console.ForegroundColor = DefaultColor;
#endif
	}

	private void PrintError(string message) => PrintError(new Error(message));

	private void PrintError(Error error)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.WriteLine(error.Message);
		Console.ForegroundColor = DefaultColor;
	}
}