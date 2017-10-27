using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.PlatformAbstractions;
using PalTracker;

namespace PalTrackerTests
{
    public static class IntegrationTestServer
    {
        private static string ContentRoot => Path.GetFullPath(
            Path.Combine(PlatformServices.Default.Application.ApplicationBasePath, "..", "..", "..", "..", "..", "src", "PalTracker")
        );

        public static TestServer Start() => new TestServer(
            new WebHostBuilder()
                .UseContentRoot(ContentRoot)
                .UseStartup<Startup>()
        );
    }
}