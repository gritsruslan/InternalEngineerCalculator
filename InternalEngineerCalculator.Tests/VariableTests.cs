using FluentAssertions;
using InternalEngineerCalculator.Main;
using InternalEngineerCalculator.Main.Expressions;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Tests;

public class VariableTests
{

	[Theory]
	[MemberData(nameof(GetVariableData))]
	public void TestVariables(string[] variableInputs, string input, double result)
	{
		var functionManager = new FunctionManager();
		var variableManager = new VariableManager();
		functionManager.InitializeDefaultFunctions();
		variableManager.InitializeBasicVariables();

		var evaluator = new Evaluator(functionManager, variableManager);
		var assignmentHandler = new AssignmentExpressionHandler(evaluator, variableManager, functionManager);
		var lexer = new Lexer();

		for (int i = 0; i < variableInputs.Length; i++)
		{
			var tokens = lexer.Tokenize(variableInputs[i]);
			tokens.IsSuccess.Should().BeTrue();
			var expr = new Parser([..tokens.Value]).ParseAssignmentExpression();
			expr.IsSuccess.Should().BeTrue();
			var handleResult = assignmentHandler
				.HandleVariableAssignmentExpression((expr.Value as VariableAssignmentExpression)!);
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

	public static IEnumerable<object[]> GetVariableData()
	{
		yield return [new[] { "x=20" }, "x + 4", 24];
		yield return [new[] { "z = log(81,1+1+1) / 2" }, "z!", 2];
		yield return [new[] { "zxc = |-15| + 2", "d = zxc + 52" }, "d", 69];
	}
}