using static Neme.Extensions.Tests.TypeExtensionsTests;
using ArgumentInvalidException = Neme.Extensions.Contracts.ArgumentInvalidException;

namespace Neme.Extensions.Tests;

public sealed class ActivatorExtensionsTests
{
    public sealed class CreateDefaultValue
    {
        [Fact]
        public void InvalidArguments()
        {
            Assert.Throws<ArgumentNullException>("type", () => ActivatorExtensions.CreateDefaultValue(null!));
        }

        [Fact]
        public void PrimitiveTypes()
        {
            Assert.Equal(false, ActivatorExtensions.CreateDefaultValue(typeof(bool)));
            Assert.Equal('\0', ActivatorExtensions.CreateDefaultValue(typeof(char)));
            Assert.Equal(0, ActivatorExtensions.CreateDefaultValue(typeof(int)));
            Assert.Null(ActivatorExtensions.CreateDefaultValue(typeof(int?)));
            Assert.Throws<ArgumentInvalidException>("type", () => ActivatorExtensions.CreateDefaultValue(typeof(Nullable<>)));
            Assert.Null(ActivatorExtensions.CreateDefaultValue(typeof(string)));
            Assert.Null(ActivatorExtensions.CreateDefaultValue(typeof(object)));
        }

        [Fact]
        public void ArrayTypes()
        {
            Assert.Null(ActivatorExtensions.CreateDefaultValue(typeof(int[])));
            Assert.Null(ActivatorExtensions.CreateDefaultValue(typeof(int[,])));
        }

        [Fact]
        public void PointerTypes()
        {
            Assert.Null(ActivatorExtensions.CreateDefaultValue(typeof(int*)));
#if NET8_0_OR_GREATER
            Assert.Null(ActivatorExtensions.CreateDefaultValue(typeof(delegate*<int, int>)));
#endif
        }

        [Fact]
        public void ByRefTypes()
        {
            Assert.Throws<ArgumentInvalidException>("type", () => ActivatorExtensions.CreateDefaultValue(typeof(int).MakeByRefType()));
        }

        [Fact]
        public void NamedTypes()
        {
            Assert.Null(ActivatorExtensions.CreateDefaultValue(typeof(CustomClass)));
            Assert.Equal(default(CustomStruct), ActivatorExtensions.CreateDefaultValue(typeof(CustomStruct)));
            Assert.Equal(default(CustomEnum), ActivatorExtensions.CreateDefaultValue(typeof(CustomEnum)));
            Assert.Null(ActivatorExtensions.CreateDefaultValue(typeof(ICustomInterface)));
            Assert.Null(ActivatorExtensions.CreateDefaultValue(typeof(CustomDelegate)));
        }

        [Fact]
        public void OpenGenericNamedTypes()
        {
            Assert.Throws<ArgumentInvalidException>("type", () => ActivatorExtensions.CreateDefaultValue(typeof(CustomClass<>)));
            Assert.Throws<ArgumentInvalidException>("type", () => ActivatorExtensions.CreateDefaultValue(typeof(CustomStruct<>)));
            Assert.Throws<ArgumentInvalidException>("type", () => ActivatorExtensions.CreateDefaultValue(typeof(ICustomInterface<>)));
            Assert.Throws<ArgumentInvalidException>("type", () => ActivatorExtensions.CreateDefaultValue(typeof(CustomDelegate<>)));
        }

        [Fact]
        public void ClosedGenericNamedTypes()
        {
            Assert.Null(ActivatorExtensions.CreateDefaultValue(typeof(CustomClass<int>)));
            Assert.Equal(default(CustomStruct<int>), ActivatorExtensions.CreateDefaultValue(typeof(CustomStruct<int>)));
            Assert.Null(ActivatorExtensions.CreateDefaultValue(typeof(ICustomInterface<int>)));
            Assert.Null(ActivatorExtensions.CreateDefaultValue(typeof(CustomDelegate<int>)));
        }

        [Fact]
        public void TypeParameters()
        {
            Assert.Throws<ArgumentInvalidException>("type", () => ActivatorExtensions.CreateDefaultValue(typeof(Unconstrained<>).GetGenericArguments()[0]));
        }
    }
}
