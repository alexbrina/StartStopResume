using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Service>();

static async Task operation(CancellationToken token)
{
    Console.Write("started ");
    while (!token.IsCancellationRequested)
    {
        try
        {
            Console.Write(".");
            await Task.Delay(200, token);
        }
        catch (TaskCanceledException)
        {
            // if token.IsCancellationRequested then Task.Delay will throw
            // we ignore and simply let while loop finish
        }
    }
    Console.WriteLine(" stopped");
}

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/start", async (Service service) =>
{
    if (await service.TryStart(operation, app.Lifetime.ApplicationStopping))
    {
        return Results.Ok("Started");
    }
    return Results.BadRequest("Already started");
});

app.MapGet("/stop", async (Service service) =>
{
    if (await service.TryStop())
    {
        return Results.Ok("Stopped");
    }
    return Results.BadRequest("Already stopped");
});

app.Run();
