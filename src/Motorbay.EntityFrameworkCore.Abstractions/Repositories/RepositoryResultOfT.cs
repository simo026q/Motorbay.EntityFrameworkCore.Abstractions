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

    internal RepositoryResult(RepositoryResultState state, IReadOnlyCollection<RepositoryError> errors, T? value)
        : base(state, errors)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the value of <typeparamref name="T"/> returned by the operation if it was successful; otherwise, <see langword="null"/>.
    /// </summary>
    public T? GetValueOrDefault() => _value;

    /// <summary>
    /// Creates a new <see cref="RepositoryResult{T}"/> representing a successful operation.
    /// </summary>
    /// <param name="value">The value returned by the operation.</param>
    /// <returns>A new <see cref="RepositoryResult{T}"/> representing a successful operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
    public new static RepositoryResult<T> Success(T value)
    {
        ArgumentNullException.ThrowIfNull(value, nameof(value));

        return new(RepositoryResultState.Success, Array.Empty<RepositoryError>(), value);
    }

    /// <summary>
    /// Creates a new <see cref="RepositoryResult{T}"/> representing a failed operation.
    /// </summary>
    /// <param name="errors">A collection of <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult{T}"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is <see langword="null"/>.</exception>
    public new static RepositoryResult<T> Failure(IReadOnlyCollection<RepositoryError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors, nameof(errors));

        return new(RepositoryResultState.Failure, errors, default);
    }

    /// <summary>
    /// Creates a new <see cref="RepositoryResult{T}"/> representing a failed operation.
    /// </summary>
    /// <param name="error">The <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult{T}"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is <see langword="null"/>.</exception>
    public new static RepositoryResult<T> Failure(RepositoryError error)
    {
        ArgumentNullException.ThrowIfNull(error, nameof(error));

        return Failure([error]);
    }

    /// <summary>
    /// Creates a new <see cref="RepositoryResult{T}"/> representing a partially successful operation.
    /// </summary>
    /// <param name="errors">A collection of <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult{T}"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is <see langword="null"/>.</exception>
    public new static RepositoryResult<T> PartialSuccess(IReadOnlyCollection<RepositoryError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors, nameof(errors));

        return new(RepositoryResultState.PartialSuccess, errors, default);
    }

    /// <summary>
    /// Creates a new <see cref="RepositoryResult{T}"/> representing a partially successful operation.
    /// </summary>
    /// <param name="error">The <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult{T}"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is <see langword="null"/>.</exception>
    public new static RepositoryResult<T> PartialSuccess(RepositoryError error)
    {
        ArgumentNullException.ThrowIfNull(error, nameof(error));

        return PartialSuccess([error]);
    }

    /// <summary>
    /// Implicitly converts a <see cref="RepositoryResult{T}"/> to its value.
    /// </summary>
    /// <param name="result">Returns the <see cref="Value"/> of the operation if it was successful; otherwise, <see langword="null"/>.</param>
    public static implicit operator T?(RepositoryResult<T> result) => result.GetValueOrDefault();
}