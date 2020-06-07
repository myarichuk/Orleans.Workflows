using System.Dynamic;
using System.Threading.Tasks;

namespace Orleans.Workflows.Interfaces
{
    public interface IOrchestratorGrain : IGrainWithGuidKey
    {
        Task Execute(WorkflowDefinition workflow);

        Task<ActivityContext> ExecuteSingle(WorkflowActivity activity, ActivityContext context);
    }
}