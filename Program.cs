using System.Diagnostics;

namespace CsProfHomeWork8
{
    internal class Program
    {
        static void Main(string[] args)
        {
            int[] sizes = { 100_000, 1_000_000, 10_000_000 };
            foreach (var size in sizes)
            {
                Console.WriteLine($"Testing with array size: {size}");
                int[] array = new int[size];
                Random random = new Random();
                for (int i = 0; i < size; i++)
                {
                    array[i] = random.Next(1, 100);
                }

                // Sequential
                Stopwatch stopwatch = Stopwatch.StartNew();
                long sum = SequentialSum(array);
                stopwatch.Stop();
                Console.WriteLine($"Sequential sum: {sum}, Time: {stopwatch.ElapsedMilliseconds} ms");

                // Threaded
                stopwatch.Restart();
                sum = ThreadedSum(array);
                stopwatch.Stop();
                Console.WriteLine($"Threaded sum: {sum}, Time: {stopwatch.ElapsedMilliseconds} ms");

                // PLINQ
                stopwatch.Restart();
                sum = ParallelLinqSum(array);
                stopwatch.Stop();
                Console.WriteLine($"PLINQ sum: {sum}, Time: {stopwatch.ElapsedMilliseconds} ms");
            }
        }
        static long SequentialSum(int[] array)
        {
            return array.Sum();
        }

        static long ThreadedSum(int[] array)
        {
            long sum = 0;
            int numberOfThreads = Environment.ProcessorCount;
            int chunkSize = array.Length / numberOfThreads;
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < numberOfThreads; i++)
            {
                int start = i * chunkSize;
                int end = (i == numberOfThreads - 1) ? array.Length : start + chunkSize;
                threads.Add(new Thread(() => {
                    for (int j = start; j < end; j++)
                    {
                        Interlocked.Add(ref sum, array[j]);
                    }
                }));
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return sum;
        }

        static long ParallelLinqSum(int[] array)
        {
            return array.AsParallel().Sum(x => (long)x);
        }
    }
}