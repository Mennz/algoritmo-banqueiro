using System;
using System.Threading;

class Program
{
    const int NUMERO_DE_CLIENTES  = 5;
    const int NUMERO_DE_RECURSOS  = 3;

    static int[]   disponivel  = new int[NUMERO_DE_RECURSOS];
    static int[,]  maximo      = new int[NUMERO_DE_CLIENTES, NUMERO_DE_RECURSOS];
    static int[,]  alocacao    = new int[NUMERO_DE_CLIENTES, NUMERO_DE_RECURSOS];
    static int[,]  necessidade = new int[NUMERO_DE_CLIENTES, NUMERO_DE_RECURSOS];

    static readonly object travarObj = new object();
    static Random aleatorio = new Random();

    static void Main(string[] args)
    {
        if (args.Length < NUMERO_DE_RECURSOS)
        {
            Console.WriteLine("Uso: dotnet run <r1> <r2> <r3>");
            return;
        }

        for (int i = 0; i < NUMERO_DE_RECURSOS; i++)
            disponivel[i] = int.Parse(args[i]);

        for (int i = 0; i < NUMERO_DE_CLIENTES; i++)
        {
            for (int j = 0; j < NUMERO_DE_RECURSOS; j++)
            {
                maximo[i, j]      = aleatorio.Next(1, disponivel[j] + 1);
                necessidade[i, j] = maximo[i, j];
            }
        }

        Thread[] threads = new Thread[NUMERO_DE_CLIENTES];
        for (int i = 0; i < NUMERO_DE_CLIENTES; i++)
        {
            int idCliente = i;
            threads[i] = new Thread(() => RotinaDoCliente(idCliente));
            threads[i].Start();
        }

        foreach (Thread t in threads)
            t.Join();
    }

    static void RotinaDoCliente(int idCliente)
    {
        while (true)
        {
            Thread.Sleep(aleatorio.Next(500, 1500));

            int[] pedido       = new int[NUMERO_DE_RECURSOS];
            bool  querRecursos = false;

            for (int i = 0; i < NUMERO_DE_RECURSOS; i++)
            {
                pedido[i] = aleatorio.Next(0, necessidade[idCliente, i] + 1);
                if (pedido[i] > 0) querRecursos = true;
            }

            if (querRecursos)
            {
                if (RequisitarRecursos(idCliente, pedido))
                {
                    Thread.Sleep(aleatorio.Next(500, 1500));

                    int[] liberacao = new int[NUMERO_DE_RECURSOS];
                    for (int i = 0; i < NUMERO_DE_RECURSOS; i++)
                        liberacao[i] = aleatorio.Next(0, alocacao[idCliente, i] + 1);

                    LiberarRecursos(idCliente, liberacao);
                }
            }
        }
    }

    static bool RequisitarRecursos(int idCliente, int[] pedido)
    {
        lock (travarObj)
        {
            for (int i = 0; i < NUMERO_DE_RECURSOS; i++)
            {
                if (pedido[i] > necessidade[idCliente, i] || pedido[i] > disponivel[i])
                    return false;
            }

            // Simula a alocação para verificar segurança
            for (int i = 0; i < NUMERO_DE_RECURSOS; i++)
            {
                disponivel[i]       -= pedido[i];
                alocacao[idCliente, i]    += pedido[i];
                necessidade[idCliente, i] -= pedido[i];
            }

            if (EstadoSeguro())
            {
                Console.WriteLine($"Cliente {idCliente} pegou: [{string.Join(",", pedido)}]");
                return true;
            }
            else
            {
                // Desfaz a simulação — estado seria inseguro
                for (int i = 0; i < NUMERO_DE_RECURSOS; i++)
                {
                    disponivel[i]       += pedido[i];
                    alocacao[idCliente, i]    -= pedido[i];
                    necessidade[idCliente, i] += pedido[i];
                }
                Console.WriteLine($"Cliente {idCliente} negado: [{string.Join(",", pedido)}]");
                return false;
            }
        }
    }

    static void LiberarRecursos(int idCliente, int[] liberacao)
    {
        lock (travarObj)
        {
            for (int i = 0; i < NUMERO_DE_RECURSOS; i++)
            {
                disponivel[i]       += liberacao[i];
                alocacao[idCliente, i]    -= liberacao[i];
                necessidade[idCliente, i] += liberacao[i];
            }
            Console.WriteLine($"Cliente {idCliente} liberou: [{string.Join(",", liberacao)}]");
        }
    }

    static bool EstadoSeguro()
    {
        int[] trabalho = new int[NUMERO_DE_RECURSOS];
        Array.Copy(disponivel, trabalho, NUMERO_DE_RECURSOS);

        bool[] concluido = new bool[NUMERO_DE_CLIENTES];
        bool   encontrou;

        do
        {
            encontrou = false;
            for (int i = 0; i < NUMERO_DE_CLIENTES; i++)
            {
                if (!concluido[i])
                {
                    bool podeTerminar = true;
                    for (int j = 0; j < NUMERO_DE_RECURSOS; j++)
                    {
                        if (necessidade[i, j] > trabalho[j])
                        {
                            podeTerminar = false;
                            break;
                        }
                    }

                    if (podeTerminar)
                    {
                        for (int j = 0; j < NUMERO_DE_RECURSOS; j++)
                            trabalho[j] += alocacao[i, j];

                        concluido[i] = true;
                        encontrou    = true;
                    }
                }
            }
        } while (encontrou);

        for (int i = 0; i < NUMERO_DE_CLIENTES; i++)
            if (!concluido[i]) return false;

        return true;
    }
}