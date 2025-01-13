namespace TagCloud.ResultUtils;

public static class Result
{
    public static Result<T> FromValue<T>(T value) => new Result<T>(value);
    public static Result<Nothing> Success() => new Result<Nothing>(default);
    public static Result<T> FromError<T>(string error) => new Result<T>(default, error);
    public static Result<Nothing> Failure(string error) => new Result<Nothing>(default, error);

    public static Result<Nothing> ForEach<T>(this Result<T[]> result, Action<T> action)
    {
        if (!result.Success)
            return Failure(result.Error!);
        foreach (var entry in result.Value!)
        {
            action(entry);
        }
        return Success();
    }
}

public record Result<T>(T? Value, string? Error = null)
{
    public bool Success => Error == null;

    public Result<TOut> Then<TOut>(Func<T, TOut> func) =>
        Success ? Result.FromValue(func(Value!)) : Result.FromError<TOut>(Error!);
    public Result<TOut> Then<TOut>(Func<T, Result<TOut>> func) =>
        Success ? func(Value!) : Result.FromError<TOut>(Error!);

    public Result<T> Then(Action<T> action)
    {
        if (!Success)
            return this;
        action(Value!);
        return this;
    }
    
    public Result<T> ChangeError(Func<string, string> func) => Success ? this : Result.FromError<T>(func(Error!));
    public Result<T> Validate(Func<T?, bool> predicate, string error)
    {
        if (!Success)
            return this;
        return predicate(Value) ? this : Result.FromError<T>(error);
    }

    public Result<T> OnError(Action<string> action)
    {
        if (!Success)
            action(Error!);
        return this;
    }
    
    public Result<T> Try(Action action)
    {
        if (!Success)
            return this;
        try
        {
            action();
            return this;
        }
        catch (Exception e)
        {
            return Result.FromError<T>(e.Message);
        }
    }

    public Result<TOut> Try<TOut>(Func<T, TOut?> func)
    {
        if (!Success)
            return Result.FromError<TOut>(Error!);

        try
        {
            return Result.FromValue(func(Value!)!);
        }
        catch (Exception e)
        {
            return Result.FromError<TOut>(e.Message);
        }
    }

    public Result<TOut> Try<TOut>(Func<T, Result<TOut>> func)
    {
        if (!Success)
            return Result.FromError<TOut>(Error!);
        
        try
        {
            return func(Value!);
        }
        catch (Exception e)
        {
            return Result.FromError<TOut>(e.Message);
        }
    }
}