namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <summary>
/// Holds the outcome of a repository operation, including the operation's state and any errors.
/// </summary>
public class RepositoryResult
{
    /// <summary>
    /// Represents an empty collection of errors.
    /// </summary>
    protected static readonly IReadOnlyCollection<RepositoryError> EmptyErrors = [];

    /// <summary>
    /// Represents a successful operation without errors.
    /// </summary>
    public static readonly RepositoryResult Success = new(RepositoryResultState.Success, EmptyErrors);

    /// <summary>
    /// The state of the repository operation.
    /// </summary>
    public RepositoryResultState State { get; }

    /// <summary>
    /// A collection of errors encountered during the operation, if any.
    /// </summary>
    public IReadOnlyCollection<RepositoryError> Errors { get; }

    /// <summary>
    /// Indicates whether the operation was successful.
    /// </summary>
    public bool Succeeded => State == RepositoryResultState.Success;

    /// <summary>
    /// Initializes a new instance of the <see cref="RepositoryResult"/> class.
    /// </summary>
    /// <param name="state">The state of the repository operation.</param>
    /// <param name="errors">A collection of errors encountered during the operation, if any.</param>
    internal RepositoryResult(RepositoryResultState state, IReadOnlyCollection<RepositoryError> errors)
    {
        State = state;
        Errors = errors;
    }

    /// <summary>
    /// Combines the current result with another, aggregating their states and errors.
    /// </summary>
    /// <param name="other">The other repository result to combine with this one.</param>
    /// <returns>A new <see cref="RepositoryResult"/> reflecting the combination of both results.</returns>
    public RepositoryResult Aggregate(RepositoryResult other)
    {
        var state = (RepositoryResultState)Math.Max((int)State, (int)other.State);

        return new RepositoryResult(state, [..Errors.Concat(other.Errors)]);
    }

    /// <summary>
    /// Creates a new <see cref="RepositoryResult"/> representing a failed operation.
    /// </summary>
    /// <param name="errors">A collection of <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult"/> representing a failed operation.</returns>
    public static RepositoryResult Failure(IReadOnlyCollection<RepositoryError> errors) => new(RepositoryResultState.Failure, errors);
    
    /// <summary>
    /// Creates a new <see cref="RepositoryResult"/> representing a failed operation.
    /// </summary>
    /// <param name="error">The <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult"/> representing a failed operation.</returns>
    public static RepositoryResult Failure(RepositoryError error) => Failure([error]);

    /// <summary>
    /// Creates a new <see cref="RepositoryResult"/> representing a partially successful operation.
    /// </summary>
    /// <param name="errors">A collection of <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult"/> representing a failed operation.</returns>
    public static RepositoryResult PartialSuccess(IReadOnlyCollection<RepositoryError> errors) => new(RepositoryResultState.PartialSuccess, errors);
    
    /// <summary>
    /// Creates a new <see cref="RepositoryResult"/> representing a partially successful operation.
    /// </summary>
    /// <param name="error">The <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult"/> representing a failed operation.</returns>
    public static RepositoryResult PartialSuccess(RepositoryError error) => new(RepositoryResultState.PartialSuccess, [error]);

    /// <summary>
    /// Returns a string representation of the <see cref="RepositoryResult"/>.
    /// </summary>
    /// <returns>A string representation of the <see cref="RepositoryResult"/>.</returns>
    public override string ToString() 
        => $"{State} ({Errors.Count} errors)";
}

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