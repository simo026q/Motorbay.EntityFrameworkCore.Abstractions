using Microsoft.EntityFrameworkCore;
using Motorbay.EntityFrameworkCore.Abstractions.Repositories;

namespace Motorbay.EntityFrameworkCore.Abstractions.UnitTests.Repositories;

public class DatabaseRepositoryTests
{
    private class GuidUniqueEntityRepository(TestDbContext dbContext)
        : DatabaseRepository<Guid, GuidUniqueEntity>(dbContext)
    {
    }

    private readonly TestDbContext _dbContext;
    private readonly GuidUniqueEntityRepository _guidUniqueEntityRepository;

    public DatabaseRepositoryTests()
    {
        DbContextOptions options = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(nameof(DatabaseRepositoryTests))
        .Options;

        _dbContext = new TestDbContext(options);
        _guidUniqueEntityRepository = new GuidUniqueEntityRepository(_dbContext);
    }

    [Fact]
    public async Task CreateAsync_WhenEntityIsValid_CreatesEntity()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        var entity = new GuidUniqueEntity();

        // Act
        var result = await _guidUniqueEntityRepository.CreateAsync(entity);

        // Assert
        Assert.Equal(RepositoryResultState.Success, result.State);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task CreateAsync_WhenEntityIsNull_ThrowsException()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _guidUniqueEntityRepository.CreateAsync(null!));
    }

    [Fact]
    public async Task CreateRangeAsync_WhenEntitiesAreValid_CreatesEntities()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        var entities = new[]
        {
            new GuidUniqueEntity(),
            new GuidUniqueEntity(),
            new GuidUniqueEntity()
        };

        // Act
        var result = await _guidUniqueEntityRepository.CreateRangeAsync(entities);

        // Assert
        Assert.Equal(RepositoryResultState.Success, result.State);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task CreateRangeAsync_WhenEntitiesIsNull_ThrowsException()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _guidUniqueEntityRepository.CreateRangeAsync(null!));
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityExists_DeletesEntity()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        var entity = new GuidUniqueEntity();
        _dbContext.GuidUniqueEntities.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _guidUniqueEntityRepository.DeleteAsync(entity);

        // Assert
        Assert.Equal(RepositoryResultState.Success, result.State);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task DeleteAsync_WhenEntityIsNull_ThrowsException()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _guidUniqueEntityRepository.DeleteAsync(null!));
    }

    [Fact]
    public async Task DeleteByIdAsync_WhenEntityExists_DeletesEntity()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        var entity = new GuidUniqueEntity();
        _dbContext.GuidUniqueEntities.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _guidUniqueEntityRepository.DeleteByIdAsync(entity.Id);

        // Assert
        Assert.Equal(RepositoryResultState.Success, result.State);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task DeleteByIdAsync_WhenEntityDoesNotExist_ReturnsEntityNotFound()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        var entity = new GuidUniqueEntity();

        // Act
        var result = await _guidUniqueEntityRepository.DeleteByIdAsync(entity.Id);

        // Assert
        Assert.Equal(RepositoryResultState.Failure, result.State);
        Assert.Single(result.Errors);
    }

    [Fact]
    public async Task DeleteRangeAsync_WhenEntitiesAreValid_DeletesEntities()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        var entities = new[]
        {
            new GuidUniqueEntity(),
            new GuidUniqueEntity(),
            new GuidUniqueEntity()
        };
        _dbContext.GuidUniqueEntities.AddRange(entities);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _guidUniqueEntityRepository.DeleteRangeAsync(entities);

        // Assert
        Assert.Equal(RepositoryResultState.Success, result.State);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task DeleteRangeAsync_WhenEntitiesIsNull_ThrowsException()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _guidUniqueEntityRepository.DeleteRangeAsync(null!));
    }

    [Fact]
    public async Task DeleteRangeByIdAsync_WhenEntitiesAreValid_DeletesEntities()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        var entities = new[]
        {
            new GuidUniqueEntity(),
            new GuidUniqueEntity(),
            new GuidUniqueEntity()
        };
        _dbContext.GuidUniqueEntities.AddRange(entities);
        await _dbContext.SaveChangesAsync();

        // Act
        var result = await _guidUniqueEntityRepository.DeleteRangeByIdAsync(entities.Select(e => e.Id));

        // Assert
        Assert.Equal(RepositoryResultState.Success, result.State);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    public async Task DeleteRangeByIdAsync_WhenEntitiesAreNotFound_ReturnsFailure(int count)
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        var entities = Enumerable.Range(0, count).Select(_ => new GuidUniqueEntity()).ToList();

        // Act
        var result = await _guidUniqueEntityRepository.DeleteRangeByIdAsync(entities.Select(e => e.Id));

        // Assert
        Assert.Equal(RepositoryResultState.Failure, result.State);
        Assert.Equal(count, result.Errors.Count);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityIsValid_UpdatesEntity()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        var entity = new GuidUniqueEntity();
        _dbContext.GuidUniqueEntities.Add(entity);
        await _dbContext.SaveChangesAsync();

        // Act
        entity.Name = "Updated";
        var result = await _guidUniqueEntityRepository.UpdateAsync(entity);

        // Assert
        Assert.Equal(RepositoryResultState.Success, result.State);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task UpdateAsync_WhenEntityIsNull_ThrowsException()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _guidUniqueEntityRepository.UpdateAsync(null!));
    }

    [Fact]
    public async Task UpdateRangeAsync_WhenEntitiesAreValid_UpdatesEntities()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();
        var entities = new[]
        {
            new GuidUniqueEntity(),
            new GuidUniqueEntity(),
            new GuidUniqueEntity()
        };
        _dbContext.GuidUniqueEntities.AddRange(entities);
        await _dbContext.SaveChangesAsync();

        // Act
        foreach (GuidUniqueEntity entity in entities)
        {
            entity.Name = "Updated";
        }

        var result = await _guidUniqueEntityRepository.UpdateRangeAsync(entities);

        // Assert
        Assert.Equal(RepositoryResultState.Success, result.State);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task UpdateRangeAsync_WhenEntitiesIsNull_ThrowsException()
    {
        // Arrange
        _dbContext.Database.EnsureDeleted();

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await _guidUniqueEntityRepository.UpdateRangeAsync(null!));
    }
}
