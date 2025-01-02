namespace InternalEngineerCalculator.Main;

static class Program
{
	private static void Main()
	{
		InternalEngineerCalculator internalEngineerCalculator;
#if DEBUG
		internalEngineerCalculator = new InternalEngineerCalculator(true);
#else
		internalEngineerCalculator = new InternalEngineerCalculator(false);
#endif
		internalEngineerCalculator.Start();
	}
}