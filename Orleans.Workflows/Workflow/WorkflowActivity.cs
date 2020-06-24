using System;
using System.Threading.Tasks;

namespace Orleans.Workflows
{
    [Serializable]
    public abstract class WorkflowActivity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public abstract Task<ActivityContext> ExecuteAsync(ActivityContext context);
    }
}
