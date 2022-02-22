using RateLimiter.Configuration;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace RateLimiter.Validators
{
    internal class RateLimiterValidator
    {
        private readonly RequestValidator _defaultRequestValidator = new();
        private readonly ConcurrentDictionary<string, RequestValidator> _endpointRequestValidators = new();
        private readonly object _endpointLock = new();

        public bool IsRequestAllowed(string endpoint, RateLimiterConfiguration configuration)
        {
            var requestTime = DateTime.UtcNow;

            if (!_defaultRequestValidator.IsRequestAllowed(requestTime, configuration.DefaultRequestLimitCount, configuration.DefaultRequestLimitMs))
                return false;

            var endpointConfiguration = configuration.EndpointLimits.FirstOrDefault(e => e?.Endpoint != null && e.Endpoint.Equals(endpoint, StringComparison.OrdinalIgnoreCase));
            if (endpointConfiguration != null)
            {
                RequestValidator endpointRequestValidator;
                lock (_endpointLock)
                {
                    if (!_endpointRequestValidators.TryGetValue(endpoint, out endpointRequestValidator))
                    {
                        endpointRequestValidator = new RequestValidator();
                        _endpointRequestValidators[endpoint] = endpointRequestValidator;
                    }
                }

                if (!endpointRequestValidator.IsRequestAllowed(requestTime, endpointConfiguration.RequestLimitCount, endpointConfiguration.RequestLimitMs))
                    return false;
            }

            return true;
        }
    }
}
