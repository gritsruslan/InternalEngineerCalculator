namespace InternalEngineerCalculator.Main;

/// <summary> Simple exceptions logger </summary>
internal sealed class Logger
{
	private static string LogDirectory => Path.Combine(Environment.CurrentDirectory, "log");

	public void LogException(Exception e)
	{
		if (!Path.Exists(LogDirectory))
			Directory.CreateDirectory(LogDirectory);

		var logName = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss") + "_log.txt";
		var filePath = Path.Combine(LogDirectory, logName);

		string logMessage = $"[{DateTime.Now}] {e.GetType()}: {e.Message}\n{e.StackTrace}\n";

		using StreamWriter fs = new StreamWriter(filePath, true);

		fs.WriteLine(logMessage);
	}
}