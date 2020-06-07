using System.Dynamic;
using System.Threading.Tasks;

namespace Orleans.Workflows.Grains
{
    public interface IWorkerGrain : IGrainWithGuidKey
    {
        Task<ActivityContext> Execute(WorkflowActivity activity, ActivityContext context);
    }
}