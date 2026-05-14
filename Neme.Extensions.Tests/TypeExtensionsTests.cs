namespace Neme.Extensions.Tests;

public sealed class TypeExtensionsTests
{
    public sealed class IsReferenceType
    {
        [Fact]
        public void InvalidArguments()
        {
            Assert.Throws<ArgumentNullException>("type", () => TypeExtensions.IsReferenceType(null!));
        }

        [Fact]
        public void PrimitiveTypes()
        {
            Assert.False(typeof(bool).IsReferenceType());
            Assert.False(typeof(char).IsReferenceType());
            Assert.False(typeof(int).IsReferenceType());
            Assert.False(typeof(int?).IsReferenceType());
            Assert.False(typeof(Nullable<>).IsReferenceType());
            Assert.True(typeof(string).IsReferenceType());
            Assert.True(typeof(object).IsReferenceType());
        }

        [Fact]
        public void ArrayTypes()
        {
            Assert.True(typeof(int[]).IsReferenceType());
            Assert.True(typeof(int[,]).IsReferenceType());
        }

        [Fact]
        public void PointerTypes()
        {
            Assert.False(typeof(int*).IsReferenceType());
#if NET8_0_OR_GREATER
            Assert.False(typeof(delegate*<int, int>).IsReferenceType());
#endif
        }

        [Fact]
        public void ByRefTypes()
        {
            Assert.False(typeof(int).MakeByRefType().IsReferenceType());
        }

        [Fact]
        public void NamedTypes()
        {
            Assert.True(typeof(CustomClass).IsReferenceType());
            Assert.False(typeof(CustomStruct).IsReferenceType());
            Assert.False(typeof(CustomEnum).IsReferenceType());
            Assert.True(typeof(ICustomInterface).IsReferenceType());
            Assert.True(typeof(CustomDelegate).IsReferenceType());
        }

        [Fact]
        public void OpenGenericNamedTypes()
        {
            Assert.True(typeof(CustomClass<>).IsReferenceType());
            Assert.False(typeof(CustomStruct<>).IsReferenceType());
            Assert.True(typeof(ICustomInterface<>).IsReferenceType());
            Assert.True(typeof(CustomDelegate<>).IsReferenceType());
        }

        [Fact]
        public void ClosedGenericNamedTypes()
        {
            Assert.True(typeof(CustomClass<int>).IsReferenceType());
            Assert.False(typeof(CustomStruct<int>).IsReferenceType());
            Assert.True(typeof(ICustomInterface<int>).IsReferenceType());
            Assert.True(typeof(CustomDelegate<int>).IsReferenceType());
        }

        [Fact]
        public void TypeParameters()
        {
            Assert.False(typeof(Unconstrained<>).GetGenericArguments()[0].IsReferenceType());
            Assert.False(typeof(Unconstrained<>.ConstrainedToT<>).GetGenericArguments()[1].IsReferenceType());
            Assert.False(typeof(Unconstrained<>.ConstrainedToT<>.ConstrainedToT2<>).GetGenericArguments()[2].IsReferenceType());
            Assert.False(typeof(ConstrainedToStruct<>).GetGenericArguments()[0].IsReferenceType());
            Assert.True(typeof(ConstrainedToClass<>).GetGenericArguments()[0].IsReferenceType());
            Assert.True(typeof(ConstrainedToClass<>.ConstrainedToT<>).GetGenericArguments()[1].IsReferenceType());
            Assert.True(typeof(ConstrainedToClass<>.ConstrainedToT<>.ConstrainedToT2<>).GetGenericArguments()[2].IsReferenceType());
            Assert.True(typeof(ConstrainedToDelegate<>).GetGenericArguments()[0].IsReferenceType());
            Assert.True(typeof(ConstrainedToDelegate<>.ConstrainedToT<>).GetGenericArguments()[1].IsReferenceType());
            Assert.True(typeof(ConstrainedToDelegate<>.ConstrainedToT<>.ConstrainedToT2<>).GetGenericArguments()[2].IsReferenceType());
            Assert.False(typeof(ConstrainedToConcreteInterface<>).GetGenericArguments()[0].IsReferenceType());
            Assert.False(typeof(ConstrainedToConcreteInterface<>.ConstrainedToT<>).GetGenericArguments()[1].IsReferenceType());
            Assert.False(typeof(ConstrainedToConcreteInterface<>.ConstrainedToT<>.ConstrainedToT2<>).GetGenericArguments()[2].IsReferenceType());
            Assert.True(typeof(ConstrainedToConcreteClass<>).GetGenericArguments()[0].IsReferenceType());
            Assert.True(typeof(ConstrainedToConcreteClass<>.ConstrainedToT<>).GetGenericArguments()[1].IsReferenceType());
            Assert.True(typeof(ConstrainedToConcreteClass<>.ConstrainedToT<>.ConstrainedToT2<>).GetGenericArguments()[2].IsReferenceType());
        }
    }

    public sealed class IsNullable
    {
        [Fact]
        public void InvalidArguments()
        {
            Assert.Throws<ArgumentNullException>("type", () => TypeExtensions.IsNullable(null!));
        }

        [Fact]
        public void PrimitiveTypes()
        {
            Assert.False(typeof(bool).IsNullable());
            Assert.False(typeof(char).IsNullable());
            Assert.False(typeof(int).IsNullable());
            Assert.True(typeof(int?).IsNullable());
            Assert.True(typeof(Nullable<>).IsNullable());
            Assert.True(typeof(string).IsNullable());
            Assert.True(typeof(object).IsNullable());
        }

        [Fact]
        public void ArrayTypes()
        {
            Assert.True(typeof(int[]).IsNullable());
            Assert.True(typeof(int[,]).IsNullable());
        }

        [Fact]
        public void PointerTypes()
        {
            Assert.True(typeof(int*).IsNullable());
#if NET8_0_OR_GREATER
            Assert.True(typeof(delegate*<int, int>).IsNullable());
#endif
        }

        [Fact]
        public void ByRefTypes()
        {
            Assert.True(typeof(int).MakeByRefType().IsNullable());
        }

        [Fact]
        public void NamedTypes()
        {
            Assert.True(typeof(CustomClass).IsNullable());
            Assert.False(typeof(CustomStruct).IsNullable());
            Assert.False(typeof(CustomEnum).IsNullable());
            Assert.True(typeof(ICustomInterface).IsNullable());
            Assert.True(typeof(CustomDelegate).IsNullable());
        }

        [Fact]
        public void OpenGenericNamedTypes()
        {
            Assert.True(typeof(CustomClass<>).IsNullable());
            Assert.False(typeof(CustomStruct<>).IsNullable());
            Assert.True(typeof(ICustomInterface<>).IsNullable());
            Assert.True(typeof(CustomDelegate<>).IsNullable());
        }

        [Fact]
        public void ClosedGenericNamedTypes()
        {
            Assert.True(typeof(CustomClass<int>).IsNullable());
            Assert.False(typeof(CustomStruct<int>).IsNullable());
            Assert.True(typeof(ICustomInterface<int>).IsNullable());
            Assert.True(typeof(CustomDelegate<int>).IsNullable());
        }

        [Fact]
        public void TypeParameters()
        {
            Assert.False(typeof(Unconstrained<>).GetGenericArguments()[0].IsNullable());
            Assert.False(typeof(Unconstrained<>.ConstrainedToT<>).GetGenericArguments()[1].IsNullable());
            Assert.False(typeof(Unconstrained<>.ConstrainedToT<>.ConstrainedToT2<>).GetGenericArguments()[2].IsNullable());
            Assert.False(typeof(ConstrainedToStruct<>).GetGenericArguments()[0].IsNullable());
            Assert.True(typeof(ConstrainedToClass<>).GetGenericArguments()[0].IsNullable());
            Assert.True(typeof(ConstrainedToClass<>.ConstrainedToT<>).GetGenericArguments()[1].IsNullable());
            Assert.True(typeof(ConstrainedToClass<>.ConstrainedToT<>.ConstrainedToT2<>).GetGenericArguments()[2].IsNullable());
            Assert.True(typeof(ConstrainedToDelegate<>).GetGenericArguments()[0].IsNullable());
            Assert.True(typeof(ConstrainedToDelegate<>.ConstrainedToT<>).GetGenericArguments()[1].IsNullable());
            Assert.True(typeof(ConstrainedToDelegate<>.ConstrainedToT<>.ConstrainedToT2<>).GetGenericArguments()[2].IsNullable());
            Assert.False(typeof(ConstrainedToConcreteInterface<>).GetGenericArguments()[0].IsNullable());
            Assert.False(typeof(ConstrainedToConcreteInterface<>.ConstrainedToT<>).GetGenericArguments()[1].IsNullable());
            Assert.False(typeof(ConstrainedToConcreteInterface<>.ConstrainedToT<>.ConstrainedToT2<>).GetGenericArguments()[2].IsNullable());
            Assert.True(typeof(ConstrainedToConcreteClass<>).GetGenericArguments()[0].IsNullable());
            Assert.True(typeof(ConstrainedToConcreteClass<>.ConstrainedToT<>).GetGenericArguments()[1].IsNullable());
            Assert.True(typeof(ConstrainedToConcreteClass<>.ConstrainedToT<>.ConstrainedToT2<>).GetGenericArguments()[2].IsNullable());
        }
    }

    public sealed class GetGenericTypeDefinitionOrSelf
    {
        [Fact]
        public void InvalidArguments()
        {
            Assert.Throws<ArgumentNullException>("type", () => TypeExtensions.GetGenericTypeDefinitionOrSelf(null!));
        }

        [Fact]
        public void PrimitiveTypes()
        {
            Assert.Equal(typeof(int), typeof(int).GetGenericTypeDefinitionOrSelf());
            Assert.Equal(typeof(object), typeof(object).GetGenericTypeDefinitionOrSelf());
        }

        [Fact]
        public void NamedTypes()
        {
            Assert.Equal(typeof(CustomClass), typeof(CustomClass).GetGenericTypeDefinitionOrSelf());
        }

        [Fact]
        public void OpenGenericNamedTypes()
        {
            Assert.Equal(typeof(CustomClass<>), typeof(CustomClass<>).GetGenericTypeDefinitionOrSelf());
            Assert.Equal(typeof(CustomClass<,>), typeof(CustomClass<,>).GetGenericTypeDefinitionOrSelf());
        }

        [Fact]
        public void ClosedGenericNamedTypes()
        {
            Assert.Equal(typeof(CustomClass<>), typeof(CustomClass<int>).GetGenericTypeDefinitionOrSelf());
            Assert.Equal(typeof(CustomClass<,>), typeof(CustomClass<object, bool>).GetGenericTypeDefinitionOrSelf());
        }

        [Fact]
        public void TypeParameters()
        {
            var typeParameter = typeof(Unconstrained<>).GetGenericArguments()[0];
            Assert.Equal(typeParameter, typeParameter.GetGenericTypeDefinitionOrSelf());
        }

        private sealed class CustomClass<T1, T2> { }
    }

#pragma warning disable CA1812 // Uninstantiated but used inside test methods
#pragma warning disable CA1852 // Cannot be made sealed because it's used in a generic constraint
    internal class CustomClass { }
#pragma warning restore CA1852
    internal sealed class CustomClass<T> { }
    internal struct CustomStruct { }
    internal struct CustomStruct<T> { }
    internal enum CustomEnum { }
    internal interface ICustomInterface { }
    internal interface ICustomInterface<T> { }
    internal delegate void CustomDelegate();
    internal delegate void CustomDelegate<T>();

    internal sealed class Unconstrained<T>
    {
        public sealed class ConstrainedToT<T2> where T2 : T
        {
            public sealed class ConstrainedToT2<T3> where T3 : T2
            {
            }
        }
    }

    internal sealed class ConstrainedToStruct<T> where T : struct
    {
    }

    internal sealed class ConstrainedToClass<T> where T : class
    {
        public sealed class ConstrainedToT<T2> where T2 : T
        {
            public sealed class ConstrainedToT2<T3> where T3 : T2
            {
            }
        }
    }

    internal sealed class ConstrainedToDelegate<T> where T : Delegate
    {
        public sealed class ConstrainedToT<T2> where T2 : T
        {
            public sealed class ConstrainedToT2<T3> where T3 : T2
            {
            }
        }
    }

    internal sealed class ConstrainedToConcreteInterface<T> where T : ICustomInterface
    {
        public sealed class ConstrainedToT<T2> where T2 : T
        {
            public sealed class ConstrainedToT2<T3> where T3 : T2
            {
            }
        }
    }

    internal sealed class ConstrainedToConcreteClass<T> where T : CustomClass
    {
        public sealed class ConstrainedToT<T2> where T2 : T
        {
            public sealed class ConstrainedToT2<T3> where T3 : T2
            {
            }
        }
    }
#pragma warning restore CA1812
}
