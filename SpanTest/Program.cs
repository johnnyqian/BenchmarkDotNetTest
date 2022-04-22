using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

BenchmarkRunner.Run<TestSpan>();

/// <summary>
/// Span<T>
/// https://mp.weixin.qq.com/s/K2YkvQnPXn6gWtICS8Zz_A
/// </summary>
public class TestSpan
{
    [Benchmark]
    public void Demo1()
    {
        var arr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        for (var i = 0; i < arr.Length; i++)
        {
            arr[i] += 1;
        }
    }

    [Benchmark]
    public void Demo2()
    {
        var arr = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
        foreach (ref var i in arr.AsSpan())
        {
            i++;
        }
    }
}