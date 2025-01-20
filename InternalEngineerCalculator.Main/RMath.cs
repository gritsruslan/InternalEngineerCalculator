using InternalEngineerCalculator.Main.Common;

namespace InternalEngineerCalculator.Main;

/// <summary> Some math functions used in program </summary>
internal static class RMath
{
	public static Result<double> Factorial(double number)
	{
		if (number < 0)
			return new Error("Factorial of negative number is not defined!");

		if (number % 1 != 0)
			return new Error("Factorial of float number is not defined!");

		double result = 1;

		for (int i = 2; i <= number; i++)
			result *= i;

		return result;
	}

	public static double Ln(double number) => Math.Log(number, Math.E);

	public static double Sqrt(double number, double degree) => Math.Exp(Ln(number) / degree);

	public static double RadiansToDegree(double radians) => radians * 180 / Math.PI;

	public static double DegreeToRadians(double degree) => degree * Math.PI / 180;
}