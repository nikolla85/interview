using Microsoft.AspNetCore.Builder;
using RateLimiter.Configuration;

namespace RateLimiter.Middlewares
{
    public static class RateLimiterMiddlewareExtension
    {
        public static IApplicationBuilder UseRateLimiter(this IApplicationBuilder builder, RateLimiterConfiguration configuration)
        {
            return builder.UseMiddleware<RateLimiterMiddleware>(configuration);
        }
    }
}
