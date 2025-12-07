using System.Diagnostics;
using OmniRepo.Application.Common.Interfaces;
using Microsoft.Extensions.Logging;

namespace OmniRepo.Application.Common.Behaviours;


public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly Stopwatch _timer = new();
    private readonly ILogger<TRequest> _logger;
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public PerformanceBehaviour(
        ILogger<TRequest> logger,
        IUser user,
        IIdentityService identityService)
    {
        _logger = logger;
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        Func<Task<TResponse>> next,
        CancellationToken cancellationToken)
    {
        _timer.Restart();

        var response = await next();

        _timer.Stop();

        var elapsed = _timer.ElapsedMilliseconds;

        if (elapsed > 500)
        {
            var requestName = typeof(TRequest).Name;
            var userId = _user.Id ?? string.Empty;

            string? userName = string.Empty;
            if (!string.IsNullOrEmpty(userId))
            {
                userName = await _identityService.GetUserNameAsync(userId);
            }

            _logger.LogWarning(
                "OmniRepo Long Running Request: {Name} ({Elapsed} ms) {UserId} {UserName} {@Request}",
                requestName,
                elapsed,
                userId,
                userName,
                request
            );
        }

        return response;
    }
}
