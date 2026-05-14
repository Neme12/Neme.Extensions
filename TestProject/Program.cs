using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using Neme.Extensions.Reflection;
using System.Reflection;
using static C;

BenchmarkRunner.Run<Benchmarks>();

public class Benchmarks
{
    private FieldInfo _field;
    private C _class;

    [GlobalSetup]
    public void Setup()
    {
        _field = typeof(C).GetField("Field", BindingFlags.Instance | BindingFlags.Public)!;
        _class = new();
    }

    [Benchmark]
    public int[] DirectGetValue()
    {
        return _class.Field;
    }

    [Benchmark]
    public int[] GetValue()
    {
        return (int[])_field.GetValue(_class)!;
    }

    [Benchmark]
    public GetterDelegate CreateDelegate()
    {
        return _field.CreateGetDelegate<C.GetterDelegate>();
    }
}

public class C
{
    public int[] Field = [];

    public delegate int[] GetterDelegate(C c);
}
