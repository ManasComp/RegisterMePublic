#region

using RegisterMe.Domain.Common;

#endregion

namespace RegisterMe.Application.Services.Ems;

public enum EmsResultValue
{
    FatalFailure,
    NormalFailure,
    Valid
}

public class EmsResult
{
    protected EmsResult(EmsResultValue resultValue, Error error)
    {
        switch (resultValue)
        {
            case EmsResultValue.Valid when error != Error.None:
                throw new InvalidOperationException("Valid result cannot have an error.");
            case EmsResultValue.NormalFailure when error == Error.None:
                throw new InvalidOperationException("NormalFailure result must have an error.");
            case EmsResultValue.FatalFailure when error == Error.None:
                throw new InvalidOperationException("Fatal failure must have an error.");
            default:
                ResultValue = resultValue;
                Error = error;
                break;
        }
    }

    public EmsResultValue ResultValue { get; }
    public bool IsValid => ResultValue == EmsResultValue.Valid;
    public bool IsFailure => IsNormalFailure || IsFatalFailure;
    public bool IsFatalFailure => ResultValue == EmsResultValue.FatalFailure;
    public bool IsNormalFailure => ResultValue == EmsResultValue.NormalFailure;
    public Error Error { get; }

    public static EmsResult Success()
    {
        return new EmsResult(EmsResultValue.Valid, Error.None);
    }

    public static EmsResult<TValue> Success<TValue>(TValue value)
    {
        return new EmsResult<TValue>(value, EmsResultValue.Valid, Error.None);
    }

    public static EmsResult Failure(Error error)
    {
        return new EmsResult(EmsResultValue.NormalFailure, error);
    }

    public static EmsResult<TValue> Failure<TValue>(Error error)
    {
        return new EmsResult<TValue>(default, EmsResultValue.NormalFailure, error);
    }

    public static EmsResult FatalFailure(Error error)
    {
        return new EmsResult(EmsResultValue.FatalFailure, error);
    }

    public static EmsResult<TValue> FatalFailure<TValue>(Error error)
    {
        return new EmsResult<TValue>(default, EmsResultValue.FatalFailure, error);
    }

    public static EmsResult<TValue> Create<TValue>(TValue? value)
    {
        return value is not null ? Success(value) : Failure<TValue>(Error.NullValue);
    }
}

public class EmsResult<TValue> : EmsResult
{
    private readonly TValue? _value;

    protected internal EmsResult(TValue? value, EmsResultValue resultValue, Error error)
        : base(resultValue, error)
    {
        _value = value;
    }

    public TValue Value => IsValid
        ? _value!
        : throw new InvalidOperationException("Cannot access the value of a failure result.");

    public static implicit operator EmsResult<TValue>(TValue? value)
    {
        return Create(value);
    }
}
