namespace Motorbay.EntityFrameworkCore.Abstractions.UnitTests.Repositories;

public class GuidUniqueEntity
    : IUniqueEntity<Guid>
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = string.Empty;
}
