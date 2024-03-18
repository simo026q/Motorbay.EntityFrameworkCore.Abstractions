
namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

public readonly struct RepositoryError(string code, string description, Exception? exception = null)
{
    public string Code { get; } = code;
    public string Description { get; } = description;
    public Exception? Exception { get; } = exception;

    internal static readonly RepositoryError UnexpectedWriteCount = new(nameof(UnexpectedWriteCount), "Unexpected write count.");
    internal static readonly RepositoryError NothingWritten = new(nameof(NothingWritten), "Nothing was written.");

    internal static RepositoryError EntityNotFound(string? id) 
        => new(nameof(EntityNotFound), $"Entity with the specified key '{id}' was not found.");

    public static RepositoryError FromException(Exception ex)
    {
        ReadOnlySpan<char> name = ex.GetType().Name;
        int idx = name.LastIndexOf("Exception", StringComparison.OrdinalIgnoreCase);
        string code = idx == -1 ? name.ToString() : name.Slice(0, idx).ToString();

        return new RepositoryError(code, ex.Message, ex);
    }
}