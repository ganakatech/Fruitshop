using System.Diagnostics;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FruitShop.Api.Behaviors;

/// <summary>
/// MediatR pipeline behavior for logging requests/responses and performance monitoring.
/// </summary>
public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

    public LoggingBehavior(ILogger<LoggingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestName = typeof(TRequest).Name;
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Handling {RequestName} at {DateTime}",
            requestName,
            DateTime.UtcNow);

        try
        {
            var response = await next();
            
            stopwatch.Stop();
            
            _logger.LogInformation(
                "Handled {RequestName} successfully in {ElapsedMilliseconds}ms at {DateTime}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                DateTime.UtcNow);

            return response;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            
            _logger.LogError(
                ex,
                "Error handling {RequestName} after {ElapsedMilliseconds}ms at {DateTime}",
                requestName,
                stopwatch.ElapsedMilliseconds,
                DateTime.UtcNow);
            
            throw;
        }
    }
}
