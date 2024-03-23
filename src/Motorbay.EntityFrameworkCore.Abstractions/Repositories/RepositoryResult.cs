namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <summary>
/// Holds the outcome of a repository operation, including the operation's state and any errors.
/// </summary>
public readonly struct RepositoryResult
{
    /// <summary>
    /// Represents a successful operation without errors.
    /// </summary>
    public static readonly RepositoryResult Success = new(RepositoryResultState.Success, []);

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

    private RepositoryResult(RepositoryResultState state, IReadOnlyCollection<RepositoryError> errors)
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
}