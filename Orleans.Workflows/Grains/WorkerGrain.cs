using System.Dynamic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Concurrency;

namespace Orleans.Workflows.Grains
{
    [StatelessWorker]
    [Reentrant]
    public class WorkerGrain : Grain, IWorkerGrain
    {
        private readonly ILogger<WorkerGrain> _logger;

        public WorkerGrain(ILogger<WorkerGrain> logger)
        {
            _logger = logger;
        }

        public async Task<ActivityContext> ExecuteAsync(WorkflowActivity activity, ActivityContext context)
        {
            await activity.ExecuteAsync(context);
            //TODO: add execution of output mapping between activity and context
            return context;
        }
    }
}
