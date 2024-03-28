namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <inheritdoc/>
/// <typeparam name="T">The type of the value returned by the operation.</typeparam>
public sealed class RepositoryResult<T>
    : RepositoryResult
    where T : class
{
    private readonly T? _value;

    /// <summary>
    /// The value returned by the operation.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the operation was unsuccessful.</exception>
    public T Value => _value ?? throw new InvalidOperationException("The operation was unsuccessful.");

    private RepositoryResult(RepositoryResultState state, IReadOnlyCollection<RepositoryError> errors, T? value)
        : base(state, errors)
    {
        _value = value;
    }

    /// <summary>
    /// Creates a new <see cref="RepositoryResult{T}"/> representing a successful operation.
    /// </summary>
    /// <param name="value">The value returned by the operation.</param>
    /// <returns>A new <see cref="RepositoryResult{T}"/> representing a successful operation.</returns>
    public new static RepositoryResult<T> Success(T value) => new(RepositoryResultState.Success, EmptyErrors, value);

    /// <summary>
    /// Creates a new <see cref="RepositoryResult{T}"/> representing a failed operation.
    /// </summary>
    /// <param name="errors">A collection of <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult{T}"/> representing a failed operation.</returns>
    public new static RepositoryResult<T> Failure(IReadOnlyCollection<RepositoryError> errors) => new(RepositoryResultState.Failure, errors, default);

    /// <summary>
    /// Creates a new <see cref="RepositoryResult{T}"/> representing a failed operation.
    /// </summary>
    /// <param name="error">The <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult{T}"/> representing a failed operation.</returns>
    public new static RepositoryResult<T> Failure(RepositoryError error) => Failure([error]);

    /// <summary>
    /// Creates a new <see cref="RepositoryResult{T}"/> representing a partially successful operation.
    /// </summary>
    /// <param name="errors">A collection of <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult{T}"/> representing a failed operation.</returns>
    public new static RepositoryResult<T> PartialSuccess(IReadOnlyCollection<RepositoryError> errors) => new(RepositoryResultState.PartialSuccess, errors, default);

    /// <summary>
    /// Creates a new <see cref="RepositoryResult{T}"/> representing a partially successful operation.
    /// </summary>
    /// <param name="error">The <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult{T}"/> representing a failed operation.</returns>
    public new static RepositoryResult<T> PartialSuccess(RepositoryError error) => new(RepositoryResultState.PartialSuccess, [error], default);

    /// <summary>
    /// Explicitly converts a <see cref="RepositoryResult{T}"/> to its value.
    /// </summary>
    /// <param name="result">Returns the <see cref="Value"/> of the operation if it was successful; otherwise, <see langword="null"/>.</param>
    public static explicit operator T?(RepositoryResult<T> result) => result._value;
}