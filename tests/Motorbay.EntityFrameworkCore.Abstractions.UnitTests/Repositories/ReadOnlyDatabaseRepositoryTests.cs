using Microsoft.EntityFrameworkCore;
using Motorbay.EntityFrameworkCore.Abstractions.Repositories;

namespace Motorbay.EntityFrameworkCore.Abstractions.UnitTests.Repositories;

public class ReadOnlyDatabaseRepositoryTests
{
    private class GuidUniqueEntityRepository(TestDbContext dbContext)
        : ReadOnlyDatabaseRepository<Guid, GuidUniqueEntity>(dbContext)
    {
    }

    private readonly TestDbContext _dbContext;
    private readonly GuidUniqueEntityRepository _guidUniqueEntityRepository;

    public ReadOnlyDatabaseRepositoryTests()
    {
        DbContextOptions options = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(nameof(ReadOnlyDatabaseRepositoryTests))
            .Options;

        _dbContext = new TestDbContext(options);
        _guidUniqueEntityRepository = new GuidUniqueEntityRepository(_dbContext);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityExists_ReturnsEntity()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();

        var entity = new GuidUniqueEntity();
        _dbContext.GuidUniqueEntities.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        RepositoryResult<GuidUniqueEntity> result = await _guidUniqueEntityRepository.GetByIdAsync(entity.Id);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal(entity, result.Value);
    }

    [Fact]
    public async Task GetByIdAsync_WhenEntityDoesNotExist_ReturnsNull()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        var entity = new GuidUniqueEntity();

        // Act
        RepositoryResult<GuidUniqueEntity> result = await _guidUniqueEntityRepository.GetByIdAsync(entity.Id);

        // Assert
        Assert.NotNull(result);
        Assert.False(result.IsSuccessful);
        Assert.Equal(RepositoryResultState.Failure, result.State);
        Assert.Throws<InvalidOperationException>(() => _ = result.Value);
    }

    [Fact]
    public async Task GetAllAsync_WhenEntitiesExist_ReturnsEntities()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();

        var entity1 = new GuidUniqueEntity();
        var entity2 = new GuidUniqueEntity();
        _dbContext.GuidUniqueEntities.AddRange(entity1, entity2);
        await _dbContext.SaveChangesAsync();

        // Act
        RepositoryResult<List<GuidUniqueEntity>> result = await _guidUniqueEntityRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.NotEmpty(result.Value);
        Assert.Equal(2, result.Value.Count);
    }

    [Fact]
    public async Task GetAllAsync_WhenNoEntitiesExist_ReturnsEmptyList()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();

        // Act
        RepositoryResult<List<GuidUniqueEntity>> result = await _guidUniqueEntityRepository.GetAllAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Empty(result.Value);
    }

    [Fact]
    public async Task ExistsAsync_WhenEntityExists_ReturnsTrue()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();

        var entity = new GuidUniqueEntity();
        _dbContext.GuidUniqueEntities.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        bool result = await _guidUniqueEntityRepository.ExistsAsync(entity.Id);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task ExistsAsync_WhenEntityDoesNotExist_ReturnsFalse()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        var entity = new GuidUniqueEntity();

        // Act
        bool result = await _guidUniqueEntityRepository.ExistsAsync(entity.Id);

        // Assert
        Assert.False(result);
    }
}
