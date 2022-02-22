using System;

namespace RateLimiter.Validators
{
    internal class RequestValidator
    {
        private int _requestCount;
        private DateTime? _firstRequestTime;

        public bool IsRequestAllowed(DateTime requestTime, int requestLimitCount, int requestLimitMs)
        {
            _firstRequestTime ??= requestTime;
            var interval = requestTime - _firstRequestTime.Value;

            if (++_requestCount > requestLimitCount && interval.TotalMilliseconds <= requestLimitMs)
                return false;

            if (interval.TotalMilliseconds > requestLimitMs)
            {
                _requestCount = 1;
                _firstRequestTime = requestTime;
            }

            return true;
        }
    }
}
