namespace InternalEngineerCalculator.Main;

/// <summary> Simple exceptions logger </summary>
internal sealed class Logger
{
	private static string LogDirectory => Path.Combine(Environment.CurrentDirectory, "log");

	public void LogException(Exception exception)
	{
#if DEBUG
#else
		if (!Path.Exists(LogDirectory))
			Directory.CreateDirectory(LogDirectory);

		var logName = DateTime.UtcNow.ToString("yyyy-MM-dd_HH::mm::ss") + "_log.txt";
		var filePath = Path.Combine(LogDirectory, logName);
		File.WriteAllText(filePath, exception.ToString());
#endif
	}
}