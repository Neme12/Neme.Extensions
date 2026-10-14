using Moq;

namespace Neme.Extensions.Ownership.Tests;

public sealed class OwnedOrBorrowed1Tests
{
    [Fact]
    public void Value_WhenOwned_ReturnsWrappedValue()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();
        var ownedOrBorrowed = new OwnedOrBorrowed<IDisposable>(mockDisposable.Object, ownsValue: true);

        // Act
        var result = ownedOrBorrowed.Value;

        // Assert
        Assert.Same(mockDisposable.Object, result);
    }

    [Fact]
    public void Value_WhenBorrowed_ReturnsWrappedValue()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();
        var ownedOrBorrowed = new OwnedOrBorrowed<IDisposable>(mockDisposable.Object, ownsValue: false);

        // Act
        var result = ownedOrBorrowed.Value;

        // Assert
        Assert.Same(mockDisposable.Object, result);
    }

    [Fact]
    public void OwnsValue_WhenOwned_ReturnsTrue()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();
        var ownedOrBorrowed = new OwnedOrBorrowed<IDisposable>(mockDisposable.Object, ownsValue: true);

        // Act
        var result = ownedOrBorrowed.OwnsValue;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void OwnsValue_WhenBorrowed_ReturnsFalse()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();
        var ownedOrBorrowed = new OwnedOrBorrowed<IDisposable>(mockDisposable.Object, ownsValue: false);

        // Act
        var result = ownedOrBorrowed.OwnsValue;

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void OwnsValue_WhenDefaultConstructor_ReturnsTrue()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();
        var ownedOrBorrowed = new OwnedOrBorrowed<IDisposable>(mockDisposable.Object);

        // Act
        var result = ownedOrBorrowed.OwnsValue;

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void Move_WhenOwned_ReturnsValueAndSetsOwnsValueToFalse()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();
        var ownedOrBorrowed = new OwnedOrBorrowed<IDisposable>(mockDisposable.Object, ownsValue: true);

        // Act
        var result = ownedOrBorrowed.Move();

        // Assert
        Assert.Same(mockDisposable.Object, result);
        Assert.False(ownedOrBorrowed.OwnsValue);
    }

    [Fact]
    public void Move_WhenBorrowed_ThrowsInvalidOperationException()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();
        var ownedOrBorrowed = new OwnedOrBorrowed<IDisposable>(mockDisposable.Object, ownsValue: false);

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => ownedOrBorrowed.Move());
        Assert.Equal("Cannot move a value that is not owned.", exception.Message);
    }

    [Fact]
    public void Dispose_WhenOwned_DisposesValueAndSetsOwnsValueToFalse()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();
        var ownedOrBorrowed = new OwnedOrBorrowed<IDisposable>(mockDisposable.Object, ownsValue: true);

        // Act
        ownedOrBorrowed.Dispose();

        // Assert
        mockDisposable.Verify(d => d.Dispose(), Times.Once);
    }

    [Fact]
    public void Dispose_WhenBorrowed_DoesNotDisposeValue()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();
        var ownedOrBorrowed = new OwnedOrBorrowed<IDisposable>(mockDisposable.Object, ownsValue: false);

        // Act
        ownedOrBorrowed.Dispose();

        // Assert
        mockDisposable.Verify(d => d.Dispose(), Times.Never);
    }

    [Fact]
    public void Dispose_WhenCalledMultipleTimes_DisposesOnlyOnce()
    {
        // Arrange
        var mockDisposable = new Mock<IDisposable>();
        var ownedOrBorrowed = new OwnedOrBorrowed<IDisposable>(mockDisposable.Object, ownsValue: true);

        // Act
        ownedOrBorrowed.Dispose();
        ownedOrBorrowed.Dispose();

        // Assert
        mockDisposable.Verify(d => d.Dispose(), Times.Once);
    }
}
