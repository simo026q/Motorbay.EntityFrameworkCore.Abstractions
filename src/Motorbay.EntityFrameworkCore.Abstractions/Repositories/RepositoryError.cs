namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <summary>
/// Represents an error that occurred during a repository operation.
/// </summary>
/// <param name="code">A code that can be used to identify the type of error.</param>
/// <param name="description">The description of the error.</param>
/// <param name="exception">An optional exception that lead to this error.</param>
public readonly struct RepositoryError(string code, string description, Exception? exception = null)
{
    /// <summary>
    /// A code that can be used to identify the type of error.
    /// </summary>
    public string Code { get; } = code;

    /// <summary>
    /// The description of the error.
    /// </summary>
    public string Description { get; } = description;

    /// <summary>
    /// An optional exception that lead to this error.
    /// </summary>
    public Exception? Exception { get; } = exception;

    /// <summary>
    /// Creates a <see cref="RepositoryError"/> from the specified <paramref name="exception"/>.
    /// </summary>
    /// <param name="exception">The exception to create the error from.</param>
    /// <returns>A new <see cref="RepositoryError"/>.</returns>
    public static RepositoryError FromException(Exception exception)
    {
        ReadOnlySpan<char> name = exception.GetType().Name;
        int idx = name.LastIndexOf("Exception", StringComparison.OrdinalIgnoreCase);
        string code = idx == -1 ? name.ToString() : name.Slice(0, idx).ToString();

        return new RepositoryError(code, exception.Message, exception);
    }
}