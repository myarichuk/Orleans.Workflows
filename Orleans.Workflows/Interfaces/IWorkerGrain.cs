using System.Threading.Tasks;

namespace Orleans.Workflows.Grains
{
    public interface IWorkerGrain : IGrainWithGuidKey
    {
        Task<ActivityContext> ExecuteAsync(WorkflowActivity activity, ActivityContext context);
    }
}