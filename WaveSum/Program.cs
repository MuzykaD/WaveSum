using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

class ParallelArraySumCalculator
{
    public static long[] TestArray { get; set; } = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
    public static int MaxThreads { get; set; } = 2;

    public static void Main(string[] args)
    {
        var random = new Random();
        var randomList = Enumerable.Range(0, 50_000_000).Select(_ => random.NextInt64(10)).ToArray();

        CalculateParallelArraySum(randomList);
        Console.WriteLine("Random list true result: " + randomList.Sum());
    }

    public static void CalculateParallelArraySum(long[] array)
    {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = MaxThreads
        };

        while (array.Length > 1)
        {
            int newArrayLength = (array.Length % 2 == 0) ? array.Length / 2 : (array.Length / 2) + 1;

            var newArray = new long[newArrayLength];

            Parallel.ForEach(Partitioner.Create(0, newArrayLength), parallelOptions, range =>
            {
                for (int i = range.Item1; i < range.Item2; i++)
                {
                    int firstNumberIndex = i;
                    int secondNumberIndex = array.Length - 1 - i;
                    long sum = secondNumberIndex >= newArrayLength ?
                              array[firstNumberIndex] + array[secondNumberIndex] : array[firstNumberIndex];

                    newArray[i] = sum;
                }
            });

            // Console.WriteLine($"Iteration #{counter++}: {PrintArray(newArray)}");
            array = newArray;
        }

        stopwatch.Stop();
        Console.WriteLine($"""
                            Final result: {array[0]}
                            Execution time: {stopwatch.Elapsed.TotalMilliseconds} ms
                            """);
    }

    private static string PrintArray(int[] array)
        => string.Join(' ', array) + "\n";
}
