using InternalEngineerCalculator.Main.Tokens;

namespace InternalEngineerCalculator.Main.Extensions;

public static class TokenExtension
{
	internal static void PrintTokens(this List<Token> tokens)
	{
		Console.WriteLine("TokenType\tValue");
		foreach (var token in tokens)
			Console.WriteLine(token.Type + "\t" + token.ValueString);
	}
}