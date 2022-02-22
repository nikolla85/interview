using Microsoft.AspNetCore.Http;
using RateLimiter.Configuration;
using RateLimiter.Validators;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading.Tasks;

namespace RateLimiter.Middlewares
{
    internal class RateLimiterMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly RateLimiterConfiguration _configuration;

        private static readonly ConcurrentDictionary<string, RateLimiterValidator> _apiRequests = new();
        private static readonly object _apiRequestLock = new();

        public RateLimiterMiddleware(RateLimiterConfiguration configuration, RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task Invoke(HttpContext context)
        {
            if (!_configuration.RequestLimiterEnabled)
            {
                await _next(context);
                return;
            }

            var endpoint = context.Request.Path.Value;
            var ipAddress = context.Connection.RemoteIpAddress.ToString();

            RateLimiterValidator validator;
            lock (_apiRequestLock)
            {
                if (!_apiRequests.TryGetValue(ipAddress, out validator))
                {
                    validator = new RateLimiterValidator();
                    _apiRequests[ipAddress] = validator;
                }
            }
         
            if (validator.IsRequestAllowed(endpoint, _configuration))
            {
                await _next(context);
                return;
            }

            context.Response.StatusCode = (int)HttpStatusCode.TooManyRequests;
        }
    }
}
