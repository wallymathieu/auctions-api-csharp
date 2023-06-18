namespace Wallymathieu.Auctions;
/// <summary>
///
/// </summary>
/// <typeparam name="TOk"></typeparam>
/// <typeparam name="TError"></typeparam>
public interface IResult<TOk, TError> : IResult
{
    IResult<TOkResult,TError> Select<TOkResult>(Func<TOk,TOkResult> map);
    IResult<TOk, TErrorResult> SelectError<TErrorResult>(Func<TError, TErrorResult> map);
}
/// <summary>
///
/// </summary>
public interface IResult
{
    public bool IsOk { get; }
    public bool IsError { get; }
}

public record Ok<TOk,TError>(TOk Value):IResult<TOk,TError>
{
    public IResult<TOkResult, TError> Select<TOkResult>(Func<TOk, TOkResult> map) =>
        new Ok<TOkResult, TError>(map(Value));

    public IResult<TOk, TErrorResult> SelectError<TErrorResult>(Func<TError, TErrorResult> map) =>
        new Ok<TOk, TErrorResult>(Value);

    public bool IsOk => true;

    public bool IsError => false;
}

public record Error<TOk,TError>(TError Value):IResult<TOk,TError>
{
    public IResult<TOkResult, TError> Select<TOkResult>(Func<TOk, TOkResult> map) =>
        new Error<TOkResult, TError>(Value);

    public IResult<TOk, TErrorResult> SelectError<TErrorResult>(Func<TError, TErrorResult> map) =>
        new Error<TOk, TErrorResult>(map(Value));

    public bool IsOk => false;

    public bool IsError => true;
}
