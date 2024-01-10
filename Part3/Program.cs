using System;
using System.Threading;

public class SingletonRandomizer
{
    private static SingletonRandomizer instance;
    private static readonly object lockObject = new object();
    private Random random;

    private SingletonRandomizer()
    {
        random = new Random();
    }

    public static SingletonRandomizer Instance
    {
        get
        {
            if (instance == null)
            {
                lock (lockObject)
                {
                    if (instance == null)
                    {
                        instance = new SingletonRandomizer();
                    }
                }
            }
            return instance;
        }
    }

    public int Next()
    {
        lock (lockObject)
        {
            return random.Next();
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        // SingletonRandomizer из разных потоков
        Thread thread1 = new Thread(() =>
        {
            var randomizer = SingletonRandomizer.Instance;
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"Thread 1: {randomizer.Next()}");
            }
        });

        Thread thread2 = new Thread(() =>
        {
            var randomizer = SingletonRandomizer.Instance;
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine($"Thread 2: {randomizer.Next()}");
            }
        });

        thread1.Start();
        thread2.Start();

        thread1.Join();
        thread2.Join();
    }
}