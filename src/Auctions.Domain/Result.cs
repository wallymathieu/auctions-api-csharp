namespace Wallymathieu.Auctions;

/// <summary>
/// This is the same type of class as can be found <a href="https://fsharp.github.io/fsharp-core-docs/reference/fsharp-core-fsharpresult-2.html">in F#</a> and <a href="https://github.com/mcintyre321/OneOf">in C#</a>.
/// The implementation of this class matches the F# implementation if you were to decompile it into C#.
/// Main reason why we want a reimplementation is to make it friendly to the C# code in this application.
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

    public static Result<TOk, TError> Ok(TOk ok)
    {
        return new Result<TOk, TError>(Tag.Ok, ok, default);
    }
    public static Result<TOk, TError> Error(TError error)
    {
        return new Result<TOk, TError>(Tag.Error, default, error);
    }
    public Result<TOkResult, TError> Select<TOkResult>(Func<TOk, TOkResult> map)
    {
        switch (_tag)
        {
            case Tag.Ok:
                return Result<TOkResult, TError>.Ok(map(_ok!));
            case Tag.Error:
                return Result<TOkResult, TError>.Error(_error!);
            default:
                throw new InvalidOperationException();
        }
    }

    public Result<TOk, TErrorResult> SelectError<TErrorResult>(Func<TError, TErrorResult> map)
    {
        switch (_tag)
        {
            case Tag.Ok:
                return Result<TOk, TErrorResult>.Ok(_ok!);
            case Tag.Error:
                return Result<TOk, TErrorResult>.Error(map(_error!));
            default:
                throw new InvalidOperationException();
        }
    }

    public void Match(Action<TOk> ok, Action<TError> error)
    {
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
    public TResult Match<TResult>(Func<TOk,TResult> ok, Func<TError,TResult> error)
    {
        switch (_tag)
        {
            case Tag.Ok:
                return ok(_ok!);
            case Tag.Error:
                return error(_error!);
            default:
                throw new InvalidOperationException();
        }
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
