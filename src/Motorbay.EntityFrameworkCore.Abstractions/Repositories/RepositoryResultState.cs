namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

/// <summary>
/// Represents the state of a repository result.
/// </summary>
/// <remarks>Lower values indicate a more successful state.</remarks>
public enum RepositoryResultState
{
    /// <summary>
    /// The operation was successful and there were no errors.
    /// </summary>
    Success,

    /// <summary>
    /// The operation was partially successful and there were non-fatal errors.
    /// </summary>
    PartialSuccess,

    /// <summary>
    /// The operation failed and there were errors.
    /// </summary>
    Failure
}