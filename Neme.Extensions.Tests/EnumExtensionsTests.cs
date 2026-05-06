using Neme.Extensions;
using Xunit;

namespace Neme.Extensions.Tests;

public class EnumExtensionsTests
{
    [Flags]
    private enum TestFlags
    {
        None = 0,
        Read = 1,
        Write = 2,
        Execute = 4,
        All = Read | Write | Execute,
    }

    private enum NonFlagsEnum
    {
        One = 1,
        Two = 2,
        Three = 3,
    }

    [Fact]
    public void HasFlag_WithMatchingFlag_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.Read | TestFlags.Write;
        var flag = TestFlags.Read;

        // Act
        var result = value.HasFlag(flag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasFlag_WithNonMatchingFlag_ReturnsFalse()
    {
        // Arrange
        var value = TestFlags.Read | TestFlags.Write;
        var flag = TestFlags.Execute;

        // Act
        var result = value.HasFlag(flag);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HasFlag_WithMultipleFlags_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.All;
        var flag = TestFlags.Read | TestFlags.Write;

        // Act
        var result = value.HasFlag(flag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasFlag_WithNoneFlag_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.Read;
        var flag = TestFlags.None;

        // Act
        var result = value.HasFlag(flag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasFlag_WithAllFlags_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.All;
        var flag = TestFlags.All;

        // Act
        var result = value.HasFlag(flag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void HasFlag_WithNoneFlagOnNone_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.None;
        var flag = TestFlags.None;

        // Act
        var result = value.HasFlag(flag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AreFlagsDefined_WithValidDefinedFlags_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.Read | TestFlags.Write;

        // Act
        var result = TestFlags.IsDefinedFlags(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AreFlagsDefined_WithNone_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.None;

        // Act
        var result = TestFlags.IsDefinedFlags(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AreFlagsDefined_WithAllFlags_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.All;

        // Act
        var result = TestFlags.IsDefinedFlags(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AreFlagsDefined_WithUndefinedFlags_ReturnsFalse()
    {
        // Arrange
        var value = (TestFlags)16;

        // Act
        var result = TestFlags.IsDefinedFlags(value);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AreFlagsDefined_WithMixedDefinedAndUndefinedFlags_ReturnsFalse()
    {
        // Arrange
        var value = TestFlags.Read | (TestFlags)16;

        // Act
        var result = TestFlags.IsDefinedFlags(value);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void AreFlagsDefined_WithNonFlagsEnum_ThrowsInvalidOperationException()
    {
        // Arrange
        var value = NonFlagsEnum.One;

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => NonFlagsEnum.IsDefinedFlags(value));
        Assert.Equal("The enum type must be a [Flags] enum.", exception.Message);
    }

    [Fact]
    public void AreFlagsDefined_WithSingleDefinedFlag_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.Read;

        // Act
        var result = TestFlags.IsDefinedFlags(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void AreFlagsDefined_WithCombinedDefinedFlags_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.Read | TestFlags.Write | TestFlags.Execute;

        // Act
        var result = TestFlags.IsDefinedFlags(value);

        // Assert
        Assert.True(result);
    }
}
