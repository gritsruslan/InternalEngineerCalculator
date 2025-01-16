using FluentAssertions;
using InternalEngineerCalculator.Main;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Tests;

public class FunctionTests
{
	public Result<double> Calculate(string command)
	{
		var tokenizeResult = new Lexer().Tokenize(command);
		if (!tokenizeResult.TryGetValue(out var tokens))
			return tokenizeResult.Error;

		var parseResult = new Parser([..tokens]).Parse();
		if (!parseResult.TryGetValue(out var expression))
			return parseResult.Error;

		var functionManager = new FunctionManager();
		functionManager.InitializeDefaultFunctions();

		var variableManager = new VariableManager();
		variableManager.InitializeBasicVariables();

		return new Evaluator(functionManager, variableManager).Evaluate(expression);
	}

	[Theory]
	[MemberData(nameof(GetBaseFunctionsData))]
	public void TestBaseFunctions(string input, double result)
	{
		var valueResult = Calculate(input);

		valueResult.IsSuccess.Should().Be(true);
		valueResult.Value.Should().BeApproximately(result, 0.00001);
	}

	[Theory]
	[MemberData(nameof(GetCustomFunctionsData))]
	public void TestCustomFunction(string[] funcInputs, string input, double result)
	{
		var functionManager = new FunctionManager();
		var variableManager = new VariableManager();
		functionManager.InitializeDefaultFunctions();
		variableManager.InitializeBasicVariables();

		var evaluator = new Evaluator(functionManager, variableManager);
		var assignmentHandler = new AssignmentExpressionHandler(evaluator, variableManager, functionManager);
		var lexer = new Lexer();

		for (int i = 0; i < funcInputs.Length; i++)
		{
			var tokens = lexer.Tokenize(funcInputs[i]);
			tokens.IsSuccess.Should().BeTrue();
			var expr = new Parser([..tokens.Value]).ParseAssignmentExpression();
			expr.IsSuccess.Should().BeTrue();
			var handleResult = assignmentHandler
				.HandleFunctionAssignmentExpression((expr.Value as FunctionAssignmentExpression)!);
			handleResult.IsSuccess.Should().BeTrue();
		}

		var tokensRes = lexer.Tokenize(input);
		tokensRes.IsSuccess.Should().BeTrue();
		var parseRes = new Parser([..tokensRes.Value]).Parse();
		parseRes.IsSuccess.Should().BeTrue();
		var resCalc = evaluator.Evaluate(parseRes.Value);
		resCalc.IsSuccess.Should().BeTrue();
		resCalc.Value.Should().Be(result);
	}

	public static IEnumerable<object[]> GetCustomFunctionsData()
	{
		yield return [new[] { "func(x) = x!" }, "func(5)", 120];
		yield return [new[] { "k(x) = 3^x + x", "f(z) = 1 + z + k(2)"}, "f(0)", 12];
		yield return [new[] {"k(x) = 10 + sin(x)", "g(x)=3*k(x)", "f(x,y)=y-g(x)"}, "f(pi,2)", -28];
	}


	public static IEnumerable<object[]> GetBaseFunctionsData()
	{
		yield return ["sin(pi)", 0];
		yield return ["cos(pi)", -1];
		yield return ["tg(1)", 1.55741];
		yield return ["ctg(1)", 0.64209];
		yield return ["sqrt(25)", 5];
		yield return ["sqrt(27,3)", 3];
		yield return ["floor(1.9)", 1];
		yield return ["ceil(1.1)", 2];
		yield return ["round(1.6)", 2];
		yield return ["rad(180)", Math.PI];
		yield return ["deg(pi)", 180];
		yield return ["log10(100)", 2];
		yield return ["log(81, 3)", 4];
		yield return ["ln(e^3)", 3];
		yield return ["exp(ln(3))", 3];
		yield return ["pow(2, -2)", 0.25];
	}
}