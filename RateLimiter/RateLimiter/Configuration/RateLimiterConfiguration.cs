using System.Collections.Generic;

namespace RateLimiter.Configuration
{
    public class RateLimiterConfiguration
    {
        public bool RequestLimiterEnabled { get; set; }
        public int DefaultRequestLimitMs { get; set; }
        public int DefaultRequestLimitCount { get; set; }
        public List<EndpointLimit> EndpointLimits { get; set; }
    }
}
