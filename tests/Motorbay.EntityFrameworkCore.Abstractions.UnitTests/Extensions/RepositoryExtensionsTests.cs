using Microsoft.EntityFrameworkCore;
using Motorbay.EntityFrameworkCore.Abstractions.Extensions;
using Motorbay.EntityFrameworkCore.Abstractions.Repositories;
using Motorbay.EntityFrameworkCore.Abstractions.UnitTests.Repositories;

namespace Motorbay.EntityFrameworkCore.Abstractions.UnitTests.Extensions;

public class RepositoryExtensionsTests
{
    private class GuidUniqueEntityRepository(TestDbContext dbContext)
        : DatabaseRepository<Guid, GuidUniqueEntity>(dbContext)
    {
    }

    private readonly TestDbContext _dbContext;
    private readonly GuidUniqueEntityRepository _guidUniqueEntityRepository;

    public RepositoryExtensionsTests()
    {
        DbContextOptions options = new DbContextOptionsBuilder()
            .UseInMemoryDatabase(nameof(RepositoryExtensionsTests))
        .Options;

        _dbContext = new TestDbContext(options);
        _guidUniqueEntityRepository = new GuidUniqueEntityRepository(_dbContext);
    }

    [Fact]
    public async Task TryCreateAsync_EntityDoesNotExist_CreatesEntity()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        var key = Guid.NewGuid();

        // Act
        RepositoryResult result = await _guidUniqueEntityRepository.TryCreateAsync(key, id => new GuidUniqueEntity { Id = id });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal(1, _dbContext.GuidUniqueEntities.Count());
    }

    [Fact]
    public async Task TryCreateAsync_EntityExists_DoesNotCreateEntity()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        var key = Guid.NewGuid();
        _dbContext.GuidUniqueEntities.Add(new GuidUniqueEntity { Id = key });
        await _dbContext.SaveChangesAsync();
        bool entityCreated = false;

        // Act
        RepositoryResult result = await _guidUniqueEntityRepository.TryCreateAsync(key, id =>
        {
            entityCreated = true;
            return new GuidUniqueEntity { Id = id };
        });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal(1, _dbContext.GuidUniqueEntities.Count());
        Assert.False(entityCreated);
    }

    [Fact]
    public async Task GetOrCreateAsync_EntityExists_ReturnsEntity()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        var key = Guid.NewGuid();
        _dbContext.GuidUniqueEntities.Add(new GuidUniqueEntity { Id = key });
        await _dbContext.SaveChangesAsync();

        // Act
        RepositoryResult<GuidUniqueEntity> result = await _guidUniqueEntityRepository.GetOrCreateAsync(key, id => new GuidUniqueEntity { Id = id });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal(1, _dbContext.GuidUniqueEntities.Count());
        Assert.Equal(key, result.Value.Id);
    }

    [Fact]
    public async Task GetOrCreateAsync_EntityDoesNotExist_CreatesEntity()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        var key = Guid.NewGuid();

        // Act
        RepositoryResult<GuidUniqueEntity> result = await _guidUniqueEntityRepository.GetOrCreateAsync(key, id => new GuidUniqueEntity { Id = id });

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal(1, _dbContext.GuidUniqueEntities.Count());
        Assert.Equal(key, result.Value.Id);
    }

    [Fact]
    public async Task CreateOrUpdateAsync_EntityExists_UpdatesEntity()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        var key = Guid.NewGuid();
        _dbContext.GuidUniqueEntities.Add(new GuidUniqueEntity { Id = key });
        await _dbContext.SaveChangesAsync();
        GuidUniqueEntity entity = _dbContext.GuidUniqueEntities.First();

        // Act
        RepositoryResult result = await _guidUniqueEntityRepository.CreateOrUpdateAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal(1, _dbContext.GuidUniqueEntities.Count());
    }

    [Fact]
    public async Task CreateOrUpdateAsync_EntityDoesNotExist_CreatesEntity()
    {
        // Arrange
        await _dbContext.Database.EnsureDeletedAsync();
        var key = Guid.NewGuid();
        GuidUniqueEntity entity = new() { Id = key };

        // Act
        RepositoryResult result = await _guidUniqueEntityRepository.CreateOrUpdateAsync(entity);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsSuccessful);
        Assert.Equal(1, _dbContext.GuidUniqueEntities.Count());
    }
}
