namespace InternalEngineerCalculator.Main;

static class Program
{
	private static void Main()
	{
		var internalEngineerCalculator = new InternalEngineerCalculator();
#if DEBUG
		internalEngineerCalculator.StartDebug();
#else
		internalEngineerCalculator.Start();
#endif
	}
}