using GraphQL.Types;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.TestHost;

namespace SAHB.GraphQL.Client.TestServer
{
    public class GraphQLWebApplicationFactory<TSchema> : WebApplicationFactory<TestserverStartup<TSchema>>
        where TSchema : Schema
    {
        protected override Microsoft.AspNetCore.TestHost.TestServer CreateServer(IWebHostBuilder builder) =>
            base.CreateServer(
                builder.UseSolutionRelativeContentRoot(""));

        protected override IWebHostBuilder CreateWebHostBuilder() =>
            WebHost.CreateDefaultBuilder()
                .UseStartup<TestserverStartup<TSchema>>();
    }
}
