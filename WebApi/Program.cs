using WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<Service>();

static async Task operation(CancellationToken token)
{
    while (!token.IsCancellationRequested)
    {
        Console.Write(".");
        await Task.Delay(200, token);
    }
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
