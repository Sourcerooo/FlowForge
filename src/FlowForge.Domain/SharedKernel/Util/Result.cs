namespace FlowForge.Domain.SharedKernel.Util;

public readonly record struct Result
{
  public bool IsSuccess { get; }
  public bool IsFailure => !IsSuccess;
  public Exception? Exception { get; }

  private Result(bool isSuccess, Exception exception)
    => (IsSuccess, Exception) = (isSuccess, exception);

  public static Result Success() => new(true, default);
  public static Result Failure(Exception exception) => new(false, exception);
}

public readonly record struct Result<T>
{
  public bool IsSuccess { get; }
  public bool IsFailure => !IsSuccess;
  public T? Value { get; }
  public Exception? Exception { get; }

  private Result(bool isSuccess, T? value, Exception? exception)
    => (IsSuccess, Value, Exception) = (isSuccess, value, exception);


  public static Result<T> Success(T value) => new Result<T>(true, value, null);
  public static Result<T> Failure(Exception exception) => new Result<T>(false, default, exception);
}

