using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;

namespace Orleans.Workflows
{
    public static class SiloBuilderExtensions
    {
        public static ISiloBuilder AddWorkflows(this ISiloBuilder siloBuilder)
        {
            siloBuilder
                .ConfigureServices(services =>
                {
                })
                .ConfigureApplicationParts(cfg =>
                {
                    cfg.AddApplicationPart(Assembly.GetExecutingAssembly()).WithReferences();
                });

            return siloBuilder;
        }
    }
}
