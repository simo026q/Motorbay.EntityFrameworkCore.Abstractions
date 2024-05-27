using Motorbay.EntityFrameworkCore.Abstractions.Repositories;

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
    /// <remarks> The returned <see cref="RepositoryResult{T}"/> will only contain the value if the state is <see cref="RepositoryResultState.Success"/>.</remarks>
    public static RepositoryResult<T> WithValue<T>(this RepositoryResult repositoryResult, T value)
        where T : class
    {
        return repositoryResult.State switch
        {
            RepositoryResultState.Success => RepositoryResult<T>.Success(value),
            RepositoryResultState.Failure => RepositoryResult<T>.Failure(repositoryResult.Errors),
            RepositoryResultState.PartialSuccess => RepositoryResult<T>.PartialSuccess(repositoryResult.Errors),
            _ => throw new InvalidOperationException("Unknown repository result state."),
        };
    }
}
