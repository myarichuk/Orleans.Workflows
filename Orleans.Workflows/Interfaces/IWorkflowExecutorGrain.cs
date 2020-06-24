using System.Threading.Tasks;

namespace Orleans.Workflows.Interfaces
{
    public interface IWorkflowExecutorGrain : IGrainWithGuidKey
    {
        Task<ActivityContext> ExecuteAsync(WorkflowDefinition workflow);

        Task<ActivityContext> ExecuteSingleAsync(WorkflowActivity activity, ActivityContext context);
    }
}