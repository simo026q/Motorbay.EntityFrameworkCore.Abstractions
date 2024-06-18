using Motorbay.EntityFrameworkCore.Abstractions.Repositories;
using System.Diagnostics.CodeAnalysis;

namespace Motorbay.EntityFrameworkCore.Abstractions.Extensions;

/// <summary>
/// Extension methods for <see cref="RepositoryResult"/> and <see cref="RepositoryResult{T}"/>.
/// </summary>
public static class RepositoryResultExtensions
{
    /// <summary>
    /// Combines the current result with another, aggregating their states and errors.
    /// </summary>
    /// <param name="repositoryResult">The current repository result to combine with the other.</param>
    /// <param name="other">The other repository result to combine with this one.</param>
    /// <returns>A new <see cref="RepositoryResult"/> reflecting the combination of both results.</returns>
    public static RepositoryResult Aggregate(this RepositoryResult repositoryResult, RepositoryResult other)
    {
        var state = (RepositoryResultState)Math.Max((int)repositoryResult.State, (int)other.State);

        return new RepositoryResult(state, [..repositoryResult.Errors.Concat(other.Errors)]);
    }

    /// <summary>
    /// Take the value of the repository result and return a new repository result with the value.
    /// </summary>
    /// <typeparam name="T">The type of the value returned by the operation.</typeparam>
    /// <param name="repositoryResult">The repository result to take the state from.</param>
    /// <param name="value">The value to return in the new repository result.</param>
    /// <returns>A new <see cref="RepositoryResult{T}"/> with the same state as <paramref name="repositoryResult"/>.</returns>
    /// <exception cref="InvalidOperationException">The repository result state is unknown.</exception>
    /// <exception cref="ArgumentNullException">The <paramref name="value"/> is <see langword="null"/> when the <paramref name="repositoryResult"/> is <see cref="RepositoryResultState.Success"/>.</exception>
    /// <remarks> The returned <see cref="RepositoryResult{T}"/> will only contain the value if the state is <see cref="RepositoryResultState.Success"/>.</remarks>
    public static RepositoryResult<T> WithValue<T>(this RepositoryResult repositoryResult, T? value)
        where T : class
    {
        return repositoryResult.State switch
        {
            RepositoryResultState.Success => RepositoryResult<T>.Success(value ?? throw new ArgumentNullException(nameof(value), "The value cannot be null when the operation is successful")),
            RepositoryResultState.Failure => RepositoryResult<T>.Failure(repositoryResult.Errors),
            RepositoryResultState.PartialSuccess => RepositoryResult<T>.PartialSuccess(repositoryResult.Errors),
            _ => throw new InvalidOperationException("Unknown repository result state."),
        };
    }

    /// <summary>
    /// Determines whether the repository result has errors.
    /// </summary>
    /// <param name="repositoryResult">The repository result to check for errors.</param>
    /// <returns><see langword="true"/> if the repository result has errors; otherwise, <see langword="false"/>.</returns>
    public static bool HasErrors(this RepositoryResult repositoryResult) 
        => repositoryResult.Errors.Count != 0;

    /// <summary>
    /// Determines whether the repository result has a specific error.
    /// </summary>
    /// <param name="repositoryResult">The repository result to check for the error.</param>
    /// <param name="errorCode">The error code to check for.</param>
    /// <returns><see langword="true"/> if the repository result has the specific error; otherwise, <see langword="false"/>.</returns>
    public static bool HasError(this RepositoryResult repositoryResult, string errorCode) 
        => repositoryResult.Errors.Any(error => error.Code == errorCode);
}
