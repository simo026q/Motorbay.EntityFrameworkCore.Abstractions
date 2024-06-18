namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <summary>
/// Holds the outcome of a repository operation, including the operation's state and any errors.
/// </summary>
public class RepositoryResult
{
    /// <summary>
    /// Represents a successful operation without errors.
    /// </summary>
    public static readonly RepositoryResult Success = new(RepositoryResultState.Success, Array.Empty<RepositoryError>());

    /// <summary>
    /// The state of the repository operation.
    /// </summary>
    public RepositoryResultState State { get; }

    /// <summary>
    /// A collection of errors encountered during the operation, if any.
    /// </summary>
    public IReadOnlyCollection<RepositoryError> Errors { get; }

    /// <summary>
    /// Indicates whether the operation is successful.
    /// </summary>
    public bool IsSuccessful => State == RepositoryResultState.Success;

    /// <summary>
    /// Indicates whether the operation has failed.
    /// </summary>
    public bool HasFailed => State == RepositoryResultState.Failure;

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
    /// Creates a new <see cref="RepositoryResult"/> representing a failed operation.
    /// </summary>
    /// <param name="errors">A collection of <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is <see langword="null"/>.</exception>
    public static RepositoryResult Failure(IReadOnlyCollection<RepositoryError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors, nameof(errors));

        return new(RepositoryResultState.Failure, errors);
    }

    /// <summary>
    /// Creates a new <see cref="RepositoryResult"/> representing a failed operation.
    /// </summary>
    /// <param name="error">The <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is <see langword="null"/>.</exception>"
    public static RepositoryResult Failure(RepositoryError error)
    {
        ArgumentNullException.ThrowIfNull(error, nameof(error));

        return Failure([error]);
    }

    /// <summary>
    /// Creates a new <see cref="RepositoryResult"/> representing a partially successful operation.
    /// </summary>
    /// <param name="errors">A collection of <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="errors"/> is <see langword="null"/>.</exception>
    public static RepositoryResult PartialSuccess(IReadOnlyCollection<RepositoryError> errors)
    {
        ArgumentNullException.ThrowIfNull(errors, nameof(errors));

        return new(RepositoryResultState.PartialSuccess, errors);
    }

    /// <summary>
    /// Creates a new <see cref="RepositoryResult"/> representing a partially successful operation.
    /// </summary>
    /// <param name="error">The <see cref="RepositoryError"/> encountered during the operation.</param>
    /// <returns>A new <see cref="RepositoryResult"/> representing a failed operation.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="error"/> is <see langword="null"/>.</exception>"
    public static RepositoryResult PartialSuccess(RepositoryError error)
    {
        ArgumentNullException.ThrowIfNull(error, nameof(error));

        return PartialSuccess([error]);
    }

    /// <summary>
    /// Returns a string representation of the <see cref="RepositoryResult"/>.
    /// </summary>
    /// <returns>A string representation of the <see cref="RepositoryResult"/>.</returns>
    public override string ToString() 
        => $"{State} ({Errors.Count} errors)";
}