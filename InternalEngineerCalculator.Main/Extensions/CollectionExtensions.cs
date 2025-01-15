using System.Collections;

namespace InternalEngineerCalculator.Main.Extensions;

internal static class CollectionExtensions
{
	public static bool IsEmpty(this ICollection collection) => collection.Count == 0;
}