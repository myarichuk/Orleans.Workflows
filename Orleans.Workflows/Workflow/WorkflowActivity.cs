using System;
using System.Dynamic;
using System.Threading.Tasks;

namespace Orleans.Workflows
{
    [Serializable]
    public abstract class WorkflowActivity
    {
        public abstract Task<ActivityContext> ExecuteAsync(ActivityContext context);
    }
}
