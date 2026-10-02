namespace CambridgeExamSystem.Application.Common;

public sealed class ServiceResult<T>
{
    private ServiceResult(bool succeeded, T? value, IReadOnlyList<string> errors)
    {
        Succeeded = succeeded;
        Value = value;
        Errors = errors;
    }

    public bool Succeeded { get; }
    public T? Value { get; }
    public IReadOnlyList<string> Errors { get; }

    public static ServiceResult<T> Success(T value) => new(true, value, []);
    public static ServiceResult<T> Failure(params string[] errors) => new(false, default, errors);
    public static ServiceResult<T> Failure(IEnumerable<string> errors) => new(false, default, errors.ToList());
}
