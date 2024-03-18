namespace Motorbay.EntityFrameworkCore.Abstractions.Repositories;

public readonly struct RepositoryResult
{
    public static readonly RepositoryResult Success = new(RepositoryResultState.Success, []);

    public RepositoryResultState State { get; }
    public IReadOnlyCollection<RepositoryError> Errors { get; }

    public bool Succeeded => State == RepositoryResultState.Success;

    private RepositoryResult(RepositoryResultState state, IReadOnlyCollection<RepositoryError> errors)
    {
        State = state;
        Errors = errors;
    }

    public RepositoryResult Aggregate(RepositoryResult other)
    {
        var state = (RepositoryResultState)Math.Max((int)State, (int)other.State);

        return new RepositoryResult(state, [..Errors.Concat(other.Errors)]);
    }

    public static RepositoryResult Failure(IReadOnlyCollection<RepositoryError> errors) => new(RepositoryResultState.Failure, errors);
    public static RepositoryResult Failure(RepositoryError error) => Failure([error]);
    public static RepositoryResult Failure(Exception exception) => Failure(RepositoryError.FromException(exception));
    public static RepositoryResult PartialSuccess(IReadOnlyCollection<RepositoryError> errors) => new(RepositoryResultState.PartialSuccess, errors);
    public static RepositoryResult PartialSuccess(RepositoryError error) => new(RepositoryResultState.PartialSuccess, [error]);
}