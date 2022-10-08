Console.WriteLine("Hello, World!");

var cts = new CancellationTokenSource();

Task.Factory.StartNew(() =>
{
    Console.WriteLine("Iniciando while true");
    while (!cts.IsCancellationRequested)
    {
        Thread.Sleep(2000);
    }
    Console.WriteLine("condicao de saida acionada, tchau!");
});

Thread.Sleep(10000);

Console.WriteLine("Agora vou acionar a condicao de saida");

cts.Cancel();

Console.WriteLine("Pronto!");

Console.ReadLine();