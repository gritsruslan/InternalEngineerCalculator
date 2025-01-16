using FluentAssertions;
using InternalEngineerCalculator.Main;
using InternalEngineerCalculator.Main.Common;
using InternalEngineerCalculator.Main.Functions;
using InternalEngineerCalculator.Main.Variables;

namespace InternalEngineerCalculator.Tests;

public class ArithmeticLogicTests
{
	private FunctionManager _functionManager = new();
	private VariableManager _variableManager = new();

	public Result<double> Calculate(string command)
	{
		var tokenizeResult = new Lexer().Tokenize(command);
		if (!tokenizeResult.TryGetValue(out var tokens))
			return tokenizeResult.Error;

		var parseResult = new Parser([..tokens]).Parse();
		if (!parseResult.TryGetValue(out var expression))
			return parseResult.Error;

		return new Evaluator(_functionManager, _variableManager).Evaluate(expression);
	}

	[Theory]
	[MemberData(nameof(GetInputs))]
	public void ArithmeticTest(string input, double result)
	{
		var res = Calculate(input);

		res.IsSuccess.Should().BeTrue();
		res.Value.Should().Be(result);
	}

	public static IEnumerable<object[]> GetInputs()
	{
		yield return ["10 - 20 + 3", -7];
		yield return ["12 * 2 - 3 / 1.5", 22];
		yield return ["-12 - (10-3)/3.5", -14];
		yield return ["|-20| + |10 - 12|", 22];
		yield return ["10 % 1.5", 1];
		yield return ["13! - 3", 6_227_020_800 - 3];
		yield return ["12/5", 2.4];
	}
}