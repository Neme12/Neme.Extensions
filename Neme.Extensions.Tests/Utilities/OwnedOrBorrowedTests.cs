using Moq;
using Neme.Extensions.Utilities;
using Xunit;

namespace Neme.Extensions.Utilities.Tests;

public sealed class OwnedOrBorrowedTests
{
    [Fact]
    public void Create_WithDefaultOwnsValue_CreatesOwnedInstance()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();

        // Act
        var result = OwnedOrBorrowed.Create(mockDisposable.Object);

        // Assert
        Assert.Equal(mockDisposable.Object, result.Value);
        Assert.True(result.OwnsValue);
    }

    [Fact]
    public void Create_WithOwnsValueTrue_CreatesOwnedInstance()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();

        // Act
        var result = OwnedOrBorrowed.Create(mockDisposable.Object, ownsValue: true);

        // Assert
        Assert.Equal(mockDisposable.Object, result.Value);
        Assert.True(result.OwnsValue);
    }

    [Fact]
    public void Create_WithOwnsValueFalse_CreatesBorrowedInstance()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();

        // Act
        var result = OwnedOrBorrowed.Create(mockDisposable.Object, ownsValue: false);

        // Assert
        Assert.Equal(mockDisposable.Object, result.Value);
        Assert.False(result.OwnsValue);
    }

    [Fact]
    public void CreateOwned_Always_CreatesOwnedInstance()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();

        // Act
        var result = OwnedOrBorrowed.CreateOwned(mockDisposable.Object);

        // Assert
        Assert.Equal(mockDisposable.Object, result.Value);
        Assert.True(result.OwnsValue);
    }

    [Fact]
    public void CreateBorrowed_Always_CreatesBorrowedInstance()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();

        // Act
        var result = OwnedOrBorrowed.CreateBorrowed(mockDisposable.Object);

        // Assert
        Assert.Equal(mockDisposable.Object, result.Value);
        Assert.False(result.OwnsValue);
    }
}
