using Microsoft.AspNetCore.Http;
using System;
using System.Net;
using System.Threading.Tasks;

namespace IntegrationTests
{
    public class FakeRemoteIpAddressMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IPAddress _fakeIpAddress = IPAddress.Parse("172.30.208.1");

        public FakeRemoteIpAddressMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public async Task Invoke(HttpContext httpContext)
        {
            httpContext.Connection.RemoteIpAddress = _fakeIpAddress;

            await _next(httpContext);
        }
    }
}
