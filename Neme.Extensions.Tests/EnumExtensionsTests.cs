namespace Neme.Extensions.Tests;

public sealed class EnumExtensionsTests
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
        var result = EnumExtensions.HasFlag(value, flag);

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
        var result = EnumExtensions.HasFlag(value, flag);

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
        var result = EnumExtensions.HasFlag(value, flag);

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
        var result = EnumExtensions.HasFlag(value, flag);

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
        var result = EnumExtensions.HasFlag(value, flag);

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
        var result = EnumExtensions.HasFlag(value, flag);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDefinedFlags_WithValidDefinedFlags_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.Read | TestFlags.Write;

        // Act
        var result = Enum.IsDefinedFlags(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDefinedFlags_WithNone_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.None;

        // Act
        var result = Enum.IsDefinedFlags(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDefinedFlags_WithAllFlags_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.All;

        // Act
        var result = Enum.IsDefinedFlags(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDefinedFlags_WithUndefinedFlags_ReturnsFalse()
    {
        // Arrange
        var value = (TestFlags)16;

        // Act
        var result = Enum.IsDefinedFlags(value);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDefinedFlags_WithMixedDefinedAndUndefinedFlags_ReturnsFalse()
    {
        // Arrange
        var value = TestFlags.Read | (TestFlags)16;

        // Act
        var result = Enum.IsDefinedFlags(value);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsDefinedFlags_WithNonFlagsEnum_ThrowsInvalidOperationException()
    {
        // Arrange
        var value = NonFlagsEnum.One;

        // Act & Assert
        var exception = Assert.Throws<InvalidOperationException>(() => Enum.IsDefinedFlags(value));
        Assert.Equal("The enum type must be a [Flags] enum.", exception.Message);
    }

    [Fact]
    public void IsDefinedFlags_WithSingleDefinedFlag_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.Read;

        // Act
        var result = Enum.IsDefinedFlags(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void IsDefinedFlags_WithCombinedDefinedFlags_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.Read | TestFlags.Write | TestFlags.Execute;

        // Act
        var result = Enum.IsDefinedFlags(value);

        // Assert
        Assert.True(result);
    }
}
