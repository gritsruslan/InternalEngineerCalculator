namespace InternalEngineerCalculator.Main.Common;

/// <summary> Custom Result datatype implementation (like in Rust) </summary>
public class Result<T>
{
	private readonly T _value;

	private readonly Error _error;

	public T Value => IsSuccess ? _value :
		throw new InvalidOperationException("The value of a failure result can not be accessed.");

	public Error Error => IsFailure ? _error :
			throw new InvalidOperationException("The error of a success result can not be accessed.");
	public bool IsSuccess { get; }

	public bool IsFailure => !IsSuccess;

	public Result(Error error)
	{
		_error = error;
		_value = default!;
		IsSuccess = false;
	}

	public Result(T value)
	{
		_value = value;
		_error = default!;
		IsSuccess = true;
	}

	public bool TryGetValue(out T value)
	{
		if (IsFailure)
		{
			value = default!;
			return false;
		}

		value = _value;
		return true;
	}
	
	public static implicit operator Result<T>(T value) => new(value);

	public static implicit operator Result<T>(Error error) => new(error);
}