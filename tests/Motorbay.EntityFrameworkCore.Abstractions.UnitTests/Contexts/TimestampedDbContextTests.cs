using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Time.Testing;
using Motorbay.EntityFrameworkCore.Abstractions.Contexts;

namespace Motorbay.EntityFrameworkCore.Abstractions.UnitTests.Contexts;

public class TimestampedDbContextTests
{
    private class TestEntity 
        : ITimestampedEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }

    private class TestDbContext(DbContextOptions options, TimeProvider timeProvider) 
        : TimestampedDbContext(options, timeProvider)
    {
        public DbSet<TestEntity> TestEntities { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TestEntity>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.CreatedAt).IsRequired();
                entity.Property(e => e.UpdatedAt).IsRequired();
            });

            base.OnModelCreating(modelBuilder);
        }
    }

    private readonly TestDbContext _context;
    private readonly FakeTimeProvider _timeProvider;

    public TimestampedDbContextTests()
    {
        DbContextOptions options = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(nameof(TimestampedDbContextTests))
            .Options;

        _timeProvider = new FakeTimeProvider();

        _context = new TestDbContext(options, _timeProvider);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEntityIsAdded_CreatedAtAndUpdatedAtAreSet()
    {
        // Arrange
        var entity = new TestEntity();

        DateTimeOffset time = DateTimeOffset.UtcNow;
        _timeProvider.SetUtcNow(time);

        // Act
        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(time, entity.CreatedAt);
        Assert.Equal(time, entity.UpdatedAt);
    }

    [Fact]
    public async Task SaveChangesAsync_WhenEntityIsModified_UpdatedAtIsUpdated()
    {
        // Arrange
        var entity = new TestEntity
        {
            Name = "Created"
        };

        DateTimeOffset createdAt = DateTimeOffset.UtcNow;
        _timeProvider.SetUtcNow(createdAt);

        _context.TestEntities.Add(entity);
        await _context.SaveChangesAsync();

        DateTimeOffset lastUpdatedAt = DateTimeOffset.UtcNow.AddHours(1);
        _timeProvider.SetUtcNow(lastUpdatedAt);

        // Act
        entity.Name = "Modified";
        await _context.SaveChangesAsync();

        // Assert
        Assert.Equal(createdAt, entity.CreatedAt);
        Assert.Equal(lastUpdatedAt, entity.UpdatedAt);
    }
}
