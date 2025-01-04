using InternalEngineerCalculator.Main.Common;

namespace InternalEngineerCalculator.Main;

internal static class RMath
{
	public static Result<double> Factorial(double number)
	{
		if (number < 0)
			return new Error("Factorial of negative number is not defined!");

		if(number % 1 != 0)
			return new Error("Factorial of float number is not defined!");

		double result = 1;

		for (int i = 2; i <= number; i++)
			result *= i;

		return result;
	}
}