namespace WebApi;

public sealed class Service : IDisposable
{
    private bool running = false;
    private CancellationTokenSource? cts;
    private CancellationTokenSource? combinedCts;
    private readonly SemaphoreSlim semaphore = new(1);

    public async Task<bool> TryStart(Func<CancellationToken, Task> operation,
        CancellationToken applicationStopping)
    {
        try
        {
            await semaphore.WaitAsync(applicationStopping);
            applicationStopping.ThrowIfCancellationRequested();

            if (running)
            {
                return false;
            }

            running = true;
            cts = new CancellationTokenSource();
            combinedCts = CancellationTokenSource.CreateLinkedTokenSource(cts.Token, applicationStopping);

            _ = Task.Factory.StartNew(async () => await operation(combinedCts.Token));

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
        combinedCts?.Dispose();
    }
}