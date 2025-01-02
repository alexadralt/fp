namespace TagCloud.ResultUtils;

public static class Result
{
    public static Result<T> FromValue<T>(T value) => new Result<T>(value);
    public static Result<Nothing> Success() => new Result<Nothing>(default);
    public static Result<T> FromError<T>(string error) => new Result<T>(default, error);
    public static Result<Nothing> Failure(string error) => new Result<Nothing>(default, error);
}

public record Result<T>(T? Value, string? Error = null)
{
    public bool Success => Error == null;
}