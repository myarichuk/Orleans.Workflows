using System.Threading.Tasks;

namespace Orleans.Workflows.Interfaces
{
    public interface IWorkflowExecutorGrain : IGrainWithGuidKey
    {
        Task<ActivityContext> ExecuteAsync(WorkflowDefinition workflow);
    }
}