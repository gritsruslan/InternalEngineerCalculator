using InternalEngineerCalculator.Main.Functions;

namespace InternalEngineerCalculator.Main.Extensions;

internal static class FunctionInfoExtensions
{
	public static string ToPrettyString(this FunctionInfo functionInfo) =>
		$"{functionInfo.Name}<{functionInfo.CountOfArg}>";
}