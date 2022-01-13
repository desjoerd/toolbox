using LogAttributes;
using MediatR;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
});

builder.Services.AddMediatR(typeof(Program));
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));

var app = builder.Build();

app.MapGet("/", async (ISender sender, [FromQuery] string? name, [FromQuery] string? description) => {
    return await sender.Send(new ExampleRequest
    {
        Id = Guid.NewGuid(),
        Name = name ?? "unknown",
        Description = description ?? "Provide name via ?name= and description via ?description= query params"
    });
});

app.Run();

public record ExampleRequest : IRequest<string>
{
    [LogProperty]
    public Guid Id { get; init; }

    [LogProperty]
    public string Name { get; init; } = string.Empty;

    public string Description { get; init; } = string.Empty;
}

public partial class ExampleQueryHandler : IRequestHandler<ExampleRequest, string>
{
    private readonly ILogger _logger;

    public ExampleQueryHandler(ILogger<ExampleQueryHandler> logger)
    {
        ArgumentNullException.ThrowIfNull(logger);
        _logger = logger;
    }

    public Task<string> Handle(ExampleRequest request, CancellationToken cancellationToken)
    {
        LogHelloWorldMessage();
        return Task.FromResult($"Id: {request.Id}, Name: {request.Name}, Description: {request.Description}");
    }

    [LoggerMessage(1, LogLevel.Information, "Log from query handler")]
    private partial void LogHelloWorldMessage();
}
