using System.Reflection;
using Jurassic;
using Microsoft.Extensions.DependencyInjection;
using Orleans.Hosting;
using Orleans.Workflows.JsProxies;

namespace Orleans.Workflows
{
    public static class SiloBuilderExtensions
    {
        public static ISiloBuilder AddWorkflows(this ISiloBuilder siloBuilder)
        {
            siloBuilder
                .ConfigureServices(services =>
                {
                    services.AddSingleton(serviceProvider =>
                    {
                        var jsEngine = new ScriptEngine();

                        jsEngine.SetGlobalValue("Random", new RandomConstructor(jsEngine));

                        return jsEngine;
                    });
                })
                .ConfigureApplicationParts(cfg =>
                {
                    cfg.AddApplicationPart(Assembly.GetExecutingAssembly()).WithReferences();
                });

            return siloBuilder;
        }
    }
}
