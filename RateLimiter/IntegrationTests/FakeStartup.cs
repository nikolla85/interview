using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using TestApi;

namespace IntegrationTests
{
    public class FakeStartup : Startup
    {
        public FakeStartup(IConfiguration configuration) : base(configuration)
        {
        }

        public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<FakeRemoteIpAddressMiddleware>();
            base.Configure(app, env);
        }
    }
}
