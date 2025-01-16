using InternalEngineerCalculator.Main.Functions;

namespace InternalEngineerCalculator.Main.Extensions;

internal static class FunctionInfoExtensions
{
	public static string ToPrettyString(this FunctionInfo functionInfo)
	{
		return $"{functionInfo.Name}<{functionInfo.CountOfArg}>";
	}
}