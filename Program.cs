using System;
using System.Threading;

class Program
{
    const int NUMBER_OF_CUSTOMERS = 5;
    const int NUMBER_OF_RESOURCES = 3;

    static int[] available = new int[NUMBER_OF_RESOURCES];
    static int[,] maximum = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];
    static int[,] allocation = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];
    static int[,] need = new int[NUMBER_OF_CUSTOMERS, NUMBER_OF_RESOURCES];

    static readonly object lockObj = new object();
    static Random rand = new Random();

    static void Main(string[] args)
    {
        if (args.Length < NUMBER_OF_RESOURCES)
        {
            Console.WriteLine("Uso: dotnet run <r1> <r2> <r3>");
            return;
        }

        for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            available[i] = int.Parse(args[i]);

        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
        {
            for (int j = 0; j < NUMBER_OF_RESOURCES; j++)
            {
                maximum[i, j] = rand.Next(1, available[j] + 1);
                need[i, j] = maximum[i, j];
            }
        }

        Thread[] threads = new Thread[NUMBER_OF_CUSTOMERS];
        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
        {
            int customerId = i;
            threads[i] = new Thread(() => CustomerRoutine(customerId));
            threads[i].Start();
        }

        foreach (Thread t in threads)
            t.Join();
    }

    static void CustomerRoutine(int customerId)
    {
        while (true)
        {
            Thread.Sleep(rand.Next(500, 1500));
            
            int[] request = new int[NUMBER_OF_RESOURCES];
            bool wantsResources = false;
            
            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                request[i] = rand.Next(0, need[customerId, i] + 1);
                if (request[i] > 0) wantsResources = true;
            }

            if (wantsResources)
            {
                if (RequestResources(customerId, request))
                {
                    Thread.Sleep(rand.Next(500, 1500));
                    int[] release = new int[NUMBER_OF_RESOURCES];
                    for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
                    {
                        release[i] = rand.Next(0, allocation[customerId, i] + 1);
                    }
                    ReleaseResources(customerId, release);
                }
            }
        }
    }

    static bool RequestResources(int customerId, int[] request)
    {
        lock (lockObj)
        {
            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                if (request[i] > need[customerId, i] || request[i] > available[i])
                    return false;
            }

            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                available[i] -= request[i];
                allocation[customerId, i] += request[i];
                need[customerId, i] -= request[i];
            }

            if (IsSafe())
            {
                Console.WriteLine($"Cliente {customerId} pegou: [{string.Join(",", request)}]");
                return true;
            }
            else
            {
                for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
                {
                    available[i] += request[i];
                    allocation[customerId, i] -= request[i];
                    need[customerId, i] += request[i];
                }
                Console.WriteLine($"Cliente {customerId} negado: [{string.Join(",", request)}]");
                return false;
            }
        }
    }

    static void ReleaseResources(int customerId, int[] release)
    {
        lock (lockObj)
        {
            for (int i = 0; i < NUMBER_OF_RESOURCES; i++)
            {
                available[i] += release[i];
                allocation[customerId, i] -= release[i];
                need[customerId, i] += release[i];
            }
            Console.WriteLine($"Cliente {customerId} liberou: [{string.Join(",", release)}]");
        }
    }

    static bool IsSafe()
    {
        int[] work = new int[NUMBER_OF_RESOURCES];
        Array.Copy(available, work, NUMBER_OF_RESOURCES);
        
        bool[] finish = new bool[NUMBER_OF_CUSTOMERS];
        bool found;
        
        do
        {
            found = false;
            for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
            {
                if (!finish[i])
                {
                    bool canFinish = true;
                    for (int j = 0; j < NUMBER_OF_RESOURCES; j++)
                    {
                        if (need[i, j] > work[j])
                        {
                            canFinish = false;
                            break;
                        }
                    }

                    if (canFinish)
                    {
                        for (int j = 0; j < NUMBER_OF_RESOURCES; j++)
                            work[j] += allocation[i, j];
                        
                        finish[i] = true;
                        found = true;
                    }
                }
            }
        } while (found);

        for (int i = 0; i < NUMBER_OF_CUSTOMERS; i++)
        {
            if (!finish[i]) return false;
        }

        return true;
    }
}