namespace WebApi;

public sealed class Service : IDisposable
{
    private bool running = false;
    private CancellationTokenSource? cts;
    private readonly SemaphoreSlim semaphore = new(1);

    public async Task<bool> TryStart(Func<CancellationToken, Task> operation)
    {
        try
        {
            await semaphore.WaitAsync();

            if (running)
            {
                return false;
            }

            running = true;
            cts = new CancellationTokenSource();
            _ = Task.Factory.StartNew(async () => await operation(cts.Token));

            return true;
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task<bool> TryStop()
    {
        try
        {
            await semaphore.WaitAsync();

            if (!running)
            {
                return false;
            }

            cts?.Cancel();
            running = false;

            return true;
        }
        finally
        {
            semaphore.Release();
        }
    }

    public void Dispose()
    {
        cts?.Dispose();
    }
}