using Motorbay.EntityFrameworkCore.Abstractions.Repositories;

namespace Motorbay.EntityFrameworkCore.Abstractions.UnitTests.Repositories;

public class RepositoryResultTests
{
    [Fact]
    public void Success_ReturnsSuccessfulRepositoryResult()
    {
        // Arrange & Act
        RepositoryResult result = RepositoryResult.Success;

        // Assert
        Assert.Equal(RepositoryResultState.Success, result.State);
        Assert.Empty(result.Errors);
        Assert.True(result.IsSuccessful);
    }

    [Fact]
    public void Failure_WithErrors_ReturnsFailedRepositoryResult()
    {
        // Arrange
        var errors = new[]
        {
            new RepositoryError("Error", "Error")
        };

        // Act
        var result = RepositoryResult.Failure(errors);

        // Assert
        Assert.Equal(RepositoryResultState.Failure, result.State);
        Assert.Equal(errors, result.Errors);
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void Failure_WithError_ReturnsFailedRepositoryResult()
    {
        // Arrange
        var error = new RepositoryError("Error", "Error");

        // Act
        var result = RepositoryResult.Failure(error);

        // Assert
        Assert.Equal(RepositoryResultState.Failure, result.State);
        Assert.Single(result.Errors);
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void Failure_WithNoErrors_ReturnsFailedRepositoryResult()
    {
        // Arrange
        IReadOnlyCollection<RepositoryError> errors = [];

        // Act
        var result = RepositoryResult.Failure(errors);

        // Assert
        Assert.Equal(RepositoryResultState.Failure, result.State);
        Assert.Empty(result.Errors);
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void Failure_WithNullErrors_ThrowsArgumentNullException()
    {
        // Arrange
        static void Act()
        {
            RepositoryResult.Failure(null!);
        }

        // Act & Assert
        Assert.Throws<ArgumentNullException>(Act);
    }

    [Fact]
    public void PartialSuccess_WithErrors_ReturnsPartiallySuccessfulRepositoryResult()
    {
        // Arrange
        var errors = new[]
        {
            new RepositoryError("Error", "Error")
        };

        // Act
        var result = RepositoryResult.PartialSuccess(errors);

        // Assert
        Assert.Equal(RepositoryResultState.PartialSuccess, result.State);
        Assert.Equal(errors, result.Errors);
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void PartialSuccess_WithError_ReturnsPartiallySuccessfulRepositoryResult()
    {
        // Arrange
        var error = new RepositoryError("Error", "Error");

        // Act
        var result = RepositoryResult.PartialSuccess(error);

        // Assert
        Assert.Equal(RepositoryResultState.PartialSuccess, result.State);
        Assert.Single(result.Errors);
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void PartialSuccess_WithNoErrors_ReturnsPartiallySuccessfulRepositoryResult()
    {
        // Arrange
        IReadOnlyCollection<RepositoryError> errors = [];

        // Act
        var result = RepositoryResult.PartialSuccess(errors);

        // Assert
        Assert.Equal(RepositoryResultState.PartialSuccess, result.State);
        Assert.Empty(result.Errors);
        Assert.False(result.IsSuccessful);
    }

    [Fact]
    public void PartialSuccess_WithNullErrors_ThrowsArgumentNullException()
    {
        // Arrange
        static void Act()
        {
            RepositoryResult.PartialSuccess(null!);
        }

        // Act & Assert
        Assert.Throws<ArgumentNullException>(Act);
    }
}
