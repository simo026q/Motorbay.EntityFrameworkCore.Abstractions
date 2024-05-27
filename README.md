# Motorbay.EntityFrameworkCore.Abstractions

This library provides a set of Entity Framework Core abstractions for the unit of work pattern and other commonly used features.

## Installation

### Package Manager Console

```bash
Install-Package Motorbay.EntityFrameworkCore.Abstractions
```

## Usage

### Repository Example

#### Model
```csharp
public class MyEntity
	: IUniqueEntity<long>
{
	public long Id { get; set; }
	public string Name { get; set; }
}
```

#### Extend RepositoryErrorDescriptor

You can extend the `RepositoryErrorDescriptor` class to add custom error codes.

```csharp
public class MyRepositoryErrorDescriptor
	: RepositoryErrorDescriptor
{
	public RepositoryError EntityWithNameNotFound<TKey, TEntity>(string name)
        where TKey : IEquatable<TKey>
        where TEntity : class, IUniqueEntity<TKey>
    {
        return new RepositoryError(
            nameof(EntityWithNameNotFound), 
			$"Entity of type '{typeof(TEntity).Name}' with name '{name}' was not found."
        );
    }

	// Add more custom error codes here...
}
```

This can be passed into the constructor of the `DatabaseRepository` or `ReadOnlyDatabaseRepository` class or registered as a singleton in the DI container.

#### Repository

You can create a repository by inheriting from `DatabaseRepository<TEntity, TKey>` or `ReadOnlyDatabaseRepository<TEntity, TKey>` or manually implementing `IRepository<TEntity, TKey>` or `IReadOnlyRepository<TEntity, TKey>`.

```csharp
public class MyEntityRepository
	: DatabaseRepository<MyEntity, long>, IMyEntityRepository
{
	public MyEntityRepository(MyDbContext context, MyRepositoryErrorDescriptor errorDescriptor)
		: base(context, errorDescriptor)
	{
	}

	public async Task<RepositoryResult<MyEntity>> GetByNameAsync(string name, bool isTracked, CancellationToken cancellationToken = default)
	{
		MyEntity? entity = await GetQuerable(isTracked).FirstOrDefaultAsync(x => x.Name == name, cancellationToken);

		return entity == null
			? RepositoryResult<MyEntity>.Failure(ErrorDescriptor.EntityWithNameNotFound<long, MyEntity>(name))
			: RepositoryResult<MyEntity>.Success(entity);
	}

	public Task<RepositoryResult<MyEntity>> GetByNameAsync(string name, CancellationToken cancellationToken = default)
	{
		return GetByNameAsync(name, isTracked: false, cancellationToken); // Default to not tracking
	}

	// Add more custom methods here...
}

public interface IMyEntityRepository
	: IRepository<MyEntity, long>
{
	// This is the recommended way to add custom methods to your repository
	Task<RepositoryResult<MyEntity>> GetByNameAsync(string name, bool isTracked, CancellationToken cancellationToken = default);
	Task<RepositoryResult<MyEntity>> GetByNameAsync(string name, CancellationToken cancellationToken = default);

	// Add more custom methods here...
}
```

#### IHostApplicationBuilder Dependency Injection

Add this to your `Program.cs` file.

```csharp
builder.Services.AddDbContext<MyDbContext>(options =>
{
	// Configure your database here
});

builder.Services.AddScoped<IMyEntityRepository, MyEntityRepository>();

// OPTIONAL: Register the custom error descriptor
builder.Services.AddSingleton<MyRepositoryErrorDescriptor>();
```

### TimestampedEntity Example

#### Model
```csharp
public class MyTimestampedEntity
	: ITimestampedEntity, IUniqueEntity<long>
{
	public long Id { get; set; }
	public string Name { get; set; }
	public DateTimeOffset CreatedAt { get; set; }
	public DateTimeOffset UpdatedAt { get; set; }
}
```

#### DbContext
```csharp
public class MyDbContext
	: TimestampedDbContext
{
	public DbSet<MyTimestampedEntity> MyTimestampedEntities { get; set; }

	public MyDbContext(DbContextOptions<MyDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		modelBuilder.Entity<MyTimestampedEntity>(entity =>
		{
			entity.Property(x => x.CreatedAt).IsRequired();
			entity.Property(x => x.UpdatedAt).IsRequired();
		});
	}

	protected override void UpdateTimestamps(DateTimeOffset date)
	{
		// OPTIONAL: Override the default update behavior.
	}
}
```