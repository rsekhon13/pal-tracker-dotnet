using System;
using System.Net;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using Xunit;
using static PalTrackerTests.DbTestSupport;

namespace PalTrackerTests
{
    [Collection("Integration")]
    public class ManagementIntegrationTest
    {
        private readonly HttpClient _testClient;

        public ManagementIntegrationTest()
        {
            Environment.SetEnvironmentVariable("VCAP_SERVICES", TestDbVcapJson);
            ExecuteSql("TRUNCATE TABLE time_entries");
            _testClient = IntegrationTestServer.Start().CreateClient();
        }

        [Fact]
        public void HasHealth()
        {
            var response = _testClient.GetAsync("/health").Result;
            var responseBody = JObject.Parse(response.Content.ReadAsStringAsync().Result);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Equal("UP", responseBody["status"]);
            Assert.Equal("UP", responseBody["diskSpace"]["status"]);
            Assert.Equal("UP", responseBody["timeEntry"]["status"]);
        }

        [Fact]
        public void HasInfo()
        {
            var response = _testClient.GetAsync("/info").Result;
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}