using System.Collections.Concurrent;

namespace SharpTracer_Core.Threading;

public static class Rng
{
    private static ConcurrentDictionary<int, Random> NumberGenerators { get; } = new();

    public static Random GetThisThreadsRng()
    {
        var threadId = Environment.CurrentManagedThreadId;

        if (NumberGenerators.ContainsKey(threadId))
        {
            return NumberGenerators[threadId];
        }

        var newRandomNumberGenerator = new Random();
        
        NumberGenerators.TryAdd(threadId, newRandomNumberGenerator);

        return newRandomNumberGenerator;
    }
}