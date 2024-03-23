namespace Motorbay.EntityFrameworkCore.Abstractions;

/// <summary>
/// Entity with creation and update timestamps.
/// </summary>
public interface ITimestampedEntity
{
    /// <summary>
    /// The timestamp when the entity was created.
    /// </summary>
    DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// The timestamp when the entity was last updated.
    /// </summary>
    DateTimeOffset UpdatedAt { get; set; }
}
