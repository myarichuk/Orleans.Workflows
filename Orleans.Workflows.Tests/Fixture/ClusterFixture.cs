using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Orleans.Hosting;
using Orleans.TestingHost;

namespace Orleans.Workflows.Tests
{
    //fixture shared by all tests, so Orleans Silo will be deployed before the first tests and released after the last test
    public class ClusterFixture : IDisposable
    {
        public const string SimpleStreamName = "InMemoryStream";

        public ClusterFixture()
        {
            var builder = new TestClusterBuilder();
            Cluster = builder.AddSiloBuilderConfigurator<TestSiloConfigurator>()
                             .Build();

            Cluster.Deploy();
        }

        public void Dispose() => Cluster.StopAllSilos();

        public TestCluster Cluster { get; }
    }

    public class TestSiloConfigurator : IHostConfigurator, ISiloConfigurator
    {
        public void Configure(IHostBuilder hostBuilder)
        {
            hostBuilder.ConfigureServices(services =>
            {
                //services.AddSingleton<T, Impl>(...);
            });
        }
        public void Configure(ISiloBuilder siloBuilder)
        {
            siloBuilder.AddSimpleMessageStreamProvider(ClusterFixture.SimpleStreamName)
                .AddWorkflows()
                .AddStartupTask((serviceProvider, cancelToken) =>
                {
                    return Task.CompletedTask;
                })
                .AddMemoryGrainStorageAsDefault();
        }
    }
}
