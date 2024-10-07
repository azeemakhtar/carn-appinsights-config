using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.AspNetCore.Http;

namespace Carnegie.ApplicationInsights.Common.TelemetryInitializers
{
    /// <summary>
    /// Set the "Auth user id" field to the authenticated user from the Http context.
    /// </summary>
    /// <remarks>
    /// Source: https://stackoverflow.com/a/59321195/736684
    /// </remarks>
    public class AuthenticatedUserIdTelemetryInitializer : ITelemetryInitializer
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticatedUserIdTelemetryInitializer(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public void Initialize(ITelemetry telemetry)
        {
            if (_httpContextAccessor?.HttpContext?.User?.Identity?.IsAuthenticated == true)
                telemetry.Context.User.AuthenticatedUserId = _httpContextAccessor.HttpContext.User.Identity.Name;
        }
    }
}
