namespace InternalEngineerCalculator.Main.Exceptions;

internal class FunctionNotFoundException(string functionName, int countOfArgs)
	: CalculatorException($"Cannot find a function \"{functionName}\" with {countOfArgs} needed arguments");