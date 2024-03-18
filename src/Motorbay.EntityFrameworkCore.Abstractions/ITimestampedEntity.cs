namespace Motorbay.EntityFrameworkCore.Abstractions;

public interface ITimestampedEntity
{
    DateTimeOffset CreatedAt { get; set; }
    DateTimeOffset UpdatedAt { get; set; }
}
