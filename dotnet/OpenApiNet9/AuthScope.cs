using Microsoft.AspNetCore.Authorization;

namespace OpenApiNet9
{
    public class AuthScopeRequirement : IAuthorizationRequirement
    {
        public string Scope { get; set; }
    }


    public class AuthScopeRequirementHandler : IAuthorizationHandler
    {
        public Task HandleAsync(AuthorizationHandlerContext context)
        {
            // do auth based on requiremtns

            return Task.CompletedTask;
        }
    }
}
