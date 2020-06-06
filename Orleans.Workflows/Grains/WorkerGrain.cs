using System.Threading.Tasks;
using Jurassic;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;
using Orleans.Workflows.Workflow;

namespace Orleans.Workflows.Grains
{
    [StatelessWorker]
    public class WorkerGrain : Grain
    {
        private readonly ScriptEngine _engine;
        private readonly ILogger<WorkerGrain> _logger;

        public WorkerGrain(ScriptEngine engine, ILogger<WorkerGrain> logger)
        {
            _engine = engine;
            _logger = logger;
        }

        public Task Execute(WorkflowActivity activity)
        {
            return Task.CompletedTask;
        }
    }
}
