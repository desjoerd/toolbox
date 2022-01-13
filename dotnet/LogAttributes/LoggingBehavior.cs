using MediatR;

namespace LogAttributes;

public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        => _logger = logger;

    public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
    {
        ArgumentNullException.ThrowIfNull(request);

        using (_logger.BeginScope(LogPropertiesOfLogScope.Create(request)))
        {
            return await next().ConfigureAwait(false);
        }
    }
}