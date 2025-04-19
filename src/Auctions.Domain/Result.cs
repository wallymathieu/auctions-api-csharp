namespace Wallymathieu.Auctions;

/// <summary>
/// Static methods to create <see cref="Result{TOk, TError}"/> instances.
/// </summary>
public static class Result
{
    public static Result<TOk, TError> Ok<TOk, TError>(TOk ok)
    {
        return new Result<TOk, TError>(ok);
    }

    public static Result<TOk, TError> Error<TOk, TError>(TError error)
    {
        return new Result<TOk, TError>(error);
    }
}

/// <summary>
/// This is the same type of class as can be found <a href="https://fsharp.github.io/fsharp-core-docs/reference/fsharp-core-fsharpresult-2.html">in F#</a> and <a href="https://github.com/mcintyre321/OneOf">in C#</a>.
/// The implementation of this class matches the F# implementation if you were to de-compile it into C#.
/// Main reason why we want a re-implementation is to make it friendly to the C# code in this application.
/// </summary>
/// <typeparam name="TOk"></typeparam>
/// <typeparam name="TError"></typeparam>
public sealed class Result<TOk, TError> : IResult
{
    private readonly Tag _tag;
    private readonly TOk? _ok;
    private readonly TError? _error;

    private enum Tag
    {
        Ok,
        Error
    }

    private Result(Tag tag, TOk? ok, TError? error)
    {
        _tag = tag;
        _ok = ok;
        _error = error;
    }

    public Result(TOk ok) : this(Tag.Ok, ok, default)
    {
    }

    public Result(TError error) : this(Tag.Error, default, error)
    {
    }

    public Result<TOkResult, TError> Select<TOkResult>(Func<TOk, TOkResult> map)
    {
        ArgumentNullException.ThrowIfNull(map);
        return _tag switch
        {
            Tag.Ok => Result.Ok<TOkResult, TError>(map(_ok!)),
            Tag.Error => Result.Error<TOkResult, TError>(_error!),
            _ => throw new InvalidOperationException(),
        };
    }

    public Result<TOk, TErrorResult> SelectError<TErrorResult>(Func<TError, TErrorResult> map)
    {
        ArgumentNullException.ThrowIfNull(map);
        return _tag switch
        {
            Tag.Ok => Result.Ok<TOk, TErrorResult>(_ok!),
            Tag.Error => Result.Error<TOk, TErrorResult>(map(_error!)),
            _ => throw new InvalidOperationException(),
        };
    }

    public void Match(Action<TOk> ok, Action<TError> error)
    {
        ArgumentNullException.ThrowIfNull(ok);
        ArgumentNullException.ThrowIfNull(error);
        switch (_tag)
        {
            case Tag.Ok:
                ok(_ok!);
                break;
            case Tag.Error:
                error(_error!);
                break;
        }
    }

    public TResult Match<TResult>(Func<TOk, TResult> ok, Func<TError, TResult> error)
    {
        ArgumentNullException.ThrowIfNull(ok);
        ArgumentNullException.ThrowIfNull(error);
        return _tag switch
        {
            Tag.Ok => ok(_ok!),
            Tag.Error => error(_error!),
            _ => throw new InvalidOperationException(),
        };
    }

    public bool IsOk => _tag == Tag.Ok;
    public bool IsError => _tag == Tag.Error;
}

/// <summary>
///
/// </summary>
public interface IResult
{
    public bool IsOk { get; }
    public bool IsError { get; }
}