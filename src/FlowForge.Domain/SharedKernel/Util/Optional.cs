namespace FlowForge.Domain.SharedKernel.Util;


public readonly struct FFOptional<T>
{
  public bool HasValue { get; }
  public T Value => HasValue
      ? field
      : throw new InvalidOperationException("No value was specified.");

  public FFOptional(T value)
  {
    Value = value;
    HasValue = true;
  }

  public static implicit operator FFOptional<T>(T value) => new(value);
}
