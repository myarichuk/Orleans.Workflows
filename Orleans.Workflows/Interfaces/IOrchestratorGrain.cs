using System.Dynamic;
using System.Threading.Tasks;

namespace Orleans.Workflows.Interfaces
{
    public interface IWorkflowExecutorGrain : IGrainWithGuidKey
    {
        Task Execute(WorkflowDefinition workflow);

        Task<ActivityContext> ExecuteSingle(WorkflowActivity activity, ActivityContext context);
    }
}