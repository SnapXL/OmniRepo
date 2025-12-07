using System.Reflection;
using OmniRepo.Application.Common.Exceptions;
using OmniRepo.Application.Common.Interfaces;
using OmniRepo.Application.Common.Security;

namespace OmniRepo.Application.Common.Behaviours;

// Minimal pipeline abstraction to replace MediatR
public interface IPipelineBehavior<TRequest, TResponse>
{
    Task<TResponse> Handle(TRequest request, Func<Task<TResponse>> next, CancellationToken cancellationToken);
}

public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IUser _user;
    private readonly IIdentityService _identityService;

    public AuthorizationBehaviour(IUser user, IIdentityService identityService)
    {
        _user = user;
        _identityService = identityService;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        Func<Task<TResponse>> next,
        CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType()
            .GetCustomAttributes<AuthorizeAttribute>();

        if (authorizeAttributes.Any())
        {
            // Must be authenticated
            if (_user.Id is null)
                throw new UnauthorizedAccessException();

            // Role-based authorization
            var roleAttributes = authorizeAttributes
                .Where(a => !string.IsNullOrWhiteSpace(a.Roles));

            if (roleAttributes.Any())
            {
                var authorized = roleAttributes
                    .SelectMany(a => a.Roles.Split(','))
                    .Any(role => _user.Roles?.Contains(role) == true);

                if (!authorized)
                    throw new ForbiddenAccessException();
            }

            // Policy-based authorization
            var policyAttributes = authorizeAttributes
                .Where(a => !string.IsNullOrWhiteSpace(a.Policy));

            foreach (var policy in policyAttributes.Select(a => a.Policy))
            {
                var authorized = await _identityService.AuthorizeAsync(_user.Id, policy);
                if (!authorized)
                    throw new ForbiddenAccessException();
            }
        }

        return await next();
    }
}
