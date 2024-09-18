using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace Neme.Extensions.Tests;

public sealed partial class Optional1Tests
{
    private static readonly ConstructorInfo _serializationConstructor = typeof(Optional<int>)
        .GetConstructor(
            BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.ExactBinding | BindingFlags.DeclaredOnly,
            binder: null,
            [typeof(SerializationInfo), typeof(StreamingContext)],
            modifiers: null)!;

    [Fact]
    public void DefaultHasNoValue()
    {
        var optional = default(Optional<int>);
        Assert.False(optional.HasValue);
        var e = Assert.Throws<InvalidOperationException>(() => optional.Value);
        Assert.Equal("Optional has no value.", e.Message);

        Assert.Equal(0, optional.GetValueOrDefault());
        Assert.Equal(1, optional.GetValueOrDefault(1));

        Assert.False(optional.TryGetValue(out var value));
        Assert.Equal(0, value);

        optional.Deconstruct(out var hasValue, out value);
        Assert.False(hasValue);
        Assert.Equal(0, value);
    }

    [Fact]
    public void ConstructedHasValue()
    {
        var optional = new Optional<int>(42);
        Assert.True(optional.HasValue);
        Assert.Equal(42, optional.Value);

        Assert.Equal(42, optional.GetValueOrDefault());
        Assert.Equal(42, optional.GetValueOrDefault(1));

        Assert.True(optional.TryGetValue(out var value));
        Assert.Equal(42, value);

        optional.Deconstruct(out var hasValue, out value);
        Assert.True(hasValue);
        Assert.Equal(42, value);
    }

    [Fact]
    public void Serialization()
    {
        SerializeAndDeserialize<int>(default);
        SerializeAndDeserialize<int>(new(42));
        SerializeAndDeserialize<object?>(null);
    }

    [Fact]
    public void Serialization_Members_None()
    {
        Assert.Throws<ArgumentNullException>("info", static () =>
        {
            try
            {
                _serializationConstructor.Invoke([null, default(StreamingContext)]);
            }
            catch (TargetInvocationException e)
            {
                throw e.InnerException!;
            }
        });

        var optional = default(Optional<int>);

#pragma warning disable SYSLIB0050 // Type or member is obsolete
        var serializationInfo = new SerializationInfo(typeof(Optional<int>), new FormatterConverter());
        ((ISerializable)optional).GetObjectData(serializationInfo, default);
#pragma warning restore SYSLIB0050 // Type or member is obsolete

        Assert.Equal(0, serializationInfo.MemberCount);

        var result = _serializationConstructor.Invoke([serializationInfo, default(StreamingContext)]);
        Assert.Equal(optional, result);
    }

    [Fact]
    public void Serialization_Members_Some()
    {
        var optional = new Optional<int>(42);

#pragma warning disable SYSLIB0050 // Type or member is obsolete
        var serializationInfo = new SerializationInfo(typeof(Optional<int>), new FormatterConverter());
        ((ISerializable)optional).GetObjectData(serializationInfo, default);
#pragma warning restore SYSLIB0050 // Type or member is obsolete

        Assert.Equal(1, serializationInfo.MemberCount);
        Assert.Equal(42, serializationInfo.GetInt32("Value"));

        var result = _serializationConstructor.Invoke([serializationInfo, default(StreamingContext)]);
        Assert.Equal(optional, result);
    }

    private static void SerializeAndDeserialize<T>(Optional<T> optional)
    {
#pragma warning disable SYSLIB0011 // Type or member is obsolete
#pragma warning disable CA2300 // Do not use insecure deserializer BinaryFormatter
#pragma warning disable CA2301 // Do not call BinaryFormatter.Deserialize without first setting BinaryFormatter.Binder
        var formatter = new BinaryFormatter();
        using var stream = new MemoryStream();
        formatter.Serialize(stream, optional);
        stream.Position = 0;

        var obj = formatter.Deserialize(stream);
        Assert.Equal(optional, obj);
#pragma warning restore CA2301 // Do not call BinaryFormatter.Deserialize without first setting BinaryFormatter.Binder
#pragma warning restore CA2300 // Do not use insecure deserializer BinaryFormatter
#pragma warning restore SYSLIB0011 // Type or member is obsolete
    }

#if NETCOREAPP2_0_OR_GREATER || NET471_OR_GREATER || NETSTANDARD2_1_OR_GREATER
    [Fact]
    public void ITupleImplementation_None()
    {
        ITuple optional = default(Optional<int>);
        Assert.Equal(0, optional.Length);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => optional[0]);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => optional[1]);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => optional[-1]);
    }

    [Fact]
    public void ITupleImplementation_Some()
    {
        ITuple optional = new Optional<int>(42);
        Assert.Equal(1, optional.Length);
        Assert.Equal(42, optional[0]);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => optional[1]);
        Assert.Throws<ArgumentOutOfRangeException>("index", () => optional[-1]);
    }
#endif

    [Fact]
    public void ImplicitConversion()
    {
        Optional<int> optional = 42;
        Assert.True(optional.HasValue);
        Assert.Equal(42, optional.Value);
    }

    [Fact]
    public void ExplicitConversion()
    {
        var optional = new Optional<int>(42);
        Assert.Equal(42, (int)optional);
    }

    [Fact]
    public void None()
    {
        Assert.Equal(default, Optional<int>.None);
    }
}
