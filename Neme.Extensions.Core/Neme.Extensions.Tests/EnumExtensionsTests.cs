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
    public void FlagsDefined_WithValidDefinedFlags_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.Read | TestFlags.Write;

        // Act
        var result = Enum.FlagsDefined(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void FlagsDefined_WithNone_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.None;

        // Act
        var result = Enum.FlagsDefined(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void FlagsDefined_WithAllFlags_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.All;

        // Act
        var result = Enum.FlagsDefined(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void FlagsDefined_WithUndefinedFlags_ReturnsFalse()
    {
        // Arrange
        var value = (TestFlags)16;

        // Act
        var result = Enum.FlagsDefined(value);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void FlagsDefined_WithMixedDefinedAndUndefinedFlags_ReturnsFalse()
    {
        // Arrange
        var value = TestFlags.Read | (TestFlags)16;

        // Act
        var result = Enum.FlagsDefined(value);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void FlagsDefined_WithNonFlagsEnum_ThrowsArgumentException()
    {
        // Arrange
        var value = NonFlagsEnum.One;

        // Act & Assert
        var exception = Assert.Throws<ArgumentException2>("value", () => Enum.FlagsDefined(value));
        Assert.Equal(value, exception.ActualValue.Value);
        Assert.StartsWith("The enum type must be a [Flags] enum.", exception.Message);
    }

    [Fact]
    public void FlagsDefined_WithSingleDefinedFlag_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.Read;

        // Act
        var result = Enum.FlagsDefined(value);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void FlagsDefined_WithCombinedDefinedFlags_ReturnsTrue()
    {
        // Arrange
        var value = TestFlags.Read | TestFlags.Write | TestFlags.Execute;

        // Act
        var result = Enum.FlagsDefined(value);

        // Assert
        Assert.True(result);
    }
}
