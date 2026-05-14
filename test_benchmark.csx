using System.Reflection;
using System.Diagnostics;
using Neme.Extensions.Reflection;

public class TestClass
{
    public int Value = 42;
}

var field = typeof(TestClass).GetField(nameof(TestClass.Value))!;
var instance = new TestClass();

// Measure compilation time
var sw = Stopwatch.StartNew();
var getter = field.CreateGetDelegate<Func<TestClass, int>>();
sw.Stop();
var compilationTime = sw.Elapsed.TotalMicroseconds;

// Warm up
for (int i = 0; i < 1000; i++)
{
    _ = getter(instance);
    _ = field.GetValue(instance);
}

// Measure GetValue performance
sw.Restart();
for (int i = 0; i < 100000; i++)
{
    _ = field.GetValue(instance);
}
sw.Stop();
var getValueTime = sw.Elapsed.TotalMilliseconds;

// Measure delegate performance
sw.Restart();
for (int i = 0; i < 100000; i++)
{
    _ = getter(instance);
}
sw.Stop();
var delegateTime = sw.Elapsed.TotalMilliseconds;

Console.WriteLine($"Compilation time: {compilationTime:F2} μs");
Console.WriteLine($"GetValue (100k calls): {getValueTime:F2} ms");
Console.WriteLine($"Delegate (100k calls): {delegateTime:F2} ms");
Console.WriteLine($"Speedup: {getValueTime/delegateTime:F1}x faster");
Console.WriteLine($"Break-even point: ~{(int)(compilationTime / ((getValueTime - delegateTime) * 10))} calls");
