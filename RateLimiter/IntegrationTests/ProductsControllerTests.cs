using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TestApi;

namespace IntegrationTests
{
    public class ProductsControllerTests
    {
        private WebApplicationFactory<FakeStartup> _factory;
        private HttpClient _client;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _factory = new TestApiFactory<FakeStartup>().WithWebHostBuilder(builder =>
             {
                 builder.UseSolutionRelativeContentRoot("TestApi");

                 builder.ConfigureTestServices(services =>
                 {
                     services.AddMvc().AddApplicationPart(typeof(Startup).Assembly);
                 });
             });
            
            _client = _factory.CreateClient();
        }

        [SetUp]
        public void SetUp()
        {
            Thread.Sleep(1000);
        }

        /// <summary>
        /// Endpoint rate limit 1req 1sec. Default rate limit 5req 1sec
        /// </summary>
        [Test]
        public async Task ThreeParalelRequestsOnRateLimitedEndpoint_OneAcceptedTwoRejected()
        {
            // Act
            var taskList = new List<Task<HttpResponseMessage>>();

            taskList.Add(_client.GetAsync("/api/products/books"));
            taskList.Add(_client.GetAsync("/api/products/books"));
            taskList.Add(_client.GetAsync("/api/products/books"));
            
            var result =  await Task.WhenAll(taskList);

            //Assert
            Assert.AreEqual(1, result.Count(r => r.StatusCode == HttpStatusCode.OK));
            Assert.AreEqual(2, result.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests));
        }

        /// <summary>
        /// Endpoint rate limit 1req 1sec. Default rate limit 5req 1sec
        /// </summary>
        [Test]
        public async Task ThreeSequentialRequestsOnRateLimitedEndpoint_OneAcceptedTwoRejected()
        {
            // Act
            var response1 = await _client.GetAsync("/api/products/books");
            var response2 = await _client.GetAsync("/api/products/books");
            var response3 = await _client.GetAsync("/api/products/books");

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode);
            Assert.AreEqual(HttpStatusCode.TooManyRequests, response2.StatusCode);
            Assert.AreEqual(HttpStatusCode.TooManyRequests, response3.StatusCode);
        }

        /// <summary>
        /// Endpoint without rate limit. Default rate limit 5req 1sec
        /// </summary>
        [Test]
        public async Task ThreeParalelRequestsOnFreeEndpoint_AllThreeAccepted()
        {
            // Act
            var taskList = new List<Task<HttpResponseMessage>>();

            taskList.Add(_client.GetAsync("/api/products/photos"));
            taskList.Add(_client.GetAsync("/api/products/photos"));
            taskList.Add(_client.GetAsync("/api/products/photos"));

            var result = await Task.WhenAll(taskList);

            //Assert
            Assert.AreEqual(3, result.Count(r => r.StatusCode == HttpStatusCode.OK));
        }

        /// <summary>
        /// Endpoint without rate limit. Default rate limit 5req 1sec
        /// </summary>
        [Test]
        public async Task ThreeSequentialRequestsOnFreeEndpoint_AllThreeAccepted()
        {
            // Act
            var response1 = await _client.GetAsync("/api/products/photos");
            var response2 = await _client.GetAsync("/api/products/photos");
            var response3 = await _client.GetAsync("/api/products/photos");

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode);
            Assert.AreEqual(HttpStatusCode.OK, response3.StatusCode);
        }

        /// <summary>
        /// Endpoint without rate limit. Default rate limit 5req 1sec
        /// </summary>
        [Test]
        public async Task SixParalelRequestsOnFreeEndpoint_FiveAcceptedOneRejected()
        {
            // Act
            var taskList = new List<Task<HttpResponseMessage>>();

            for (int i = 0; i < 6; i++)
            {
                taskList.Add(_client.GetAsync("/api/products/photos"));
            }

            var result = await Task.WhenAll(taskList);

            //Assert
            Assert.AreEqual(5, result.Count(r => r.StatusCode == HttpStatusCode.OK));
            Assert.AreEqual(1, result.Count(r => r.StatusCode == HttpStatusCode.TooManyRequests));
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _client.Dispose();
            _factory.Dispose();
        }
    }
}