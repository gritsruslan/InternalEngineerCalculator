namespace InternalEngineerCalculator.Main.Common;

public class EmptyResult
{
	private readonly Error _error;

	public bool IsSuccess { get; }

	public bool IsFailure => !IsSuccess;

	public Error Error => IsFailure ? _error :
		throw new InvalidOperationException("The error of a success result can not be accessed.");

	public EmptyResult(Error error)
	{
		IsSuccess = false;
		_error = error;
	}

	public EmptyResult()
	{
		IsSuccess = true;
		_error = default!;
	}

	public static EmptyResult Success() => new();

	public static implicit operator EmptyResult(Error error) => new(error);
}