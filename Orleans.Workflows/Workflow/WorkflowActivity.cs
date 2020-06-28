using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Orleans.Workflows
{
    [Serializable]
    public abstract class WorkflowActivity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public List<Expression<Action<WorkflowActivity>>> InputSetters { get; } = new List<Expression<Action<WorkflowActivity>>>();
        public List<Expression<Action<WorkflowActivity, ActivityContext>>> InputSettersWithContext { get; } = new List<Expression<Action<WorkflowActivity, ActivityContext>>>();
        
        public abstract Task ExecuteAsync(ActivityContext context);
    }
}
