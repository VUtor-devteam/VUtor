using Microsoft.AspNetCore.Authorization;
using VUtor.Services.ExceptionTracker;

namespace VUtor.Services
{
    public class ErrorRouteAuthorization : AuthorizationHandler<IAuthorizationRequirement>
    {
        private readonly IExceptionTracker _exceptionTracker;

        public ErrorRouteAuthorization(IExceptionTracker exceptionTracker)
        {
            _exceptionTracker = exceptionTracker;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                       IAuthorizationRequirement requirement)
        {
            var httpContext = context.Resource as HttpContext;
            if (httpContext == null)
                return Task.CompletedTask;

            // Restrict access if no exception occurred
            if (httpContext.Request.Path == "/Error" && !_exceptionTracker.ExceptionOccurred)
            {
                context.Fail();
                return Task.CompletedTask;
            }

            return Task.CompletedTask;
        }
    }
}
