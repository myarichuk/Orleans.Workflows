using QuickGraph;
using System;

namespace Orleans.Workflows
{

    public class WorkflowBuilder : IWorkflowDefinitionBuilder
    {
        internal readonly AdjacencyGraph<WorkflowActivity, EdgeWithPredicate> _flow = 
            new AdjacencyGraph<WorkflowActivity, EdgeWithPredicate>(true);

        internal readonly WorkflowDefinition.IOMappingContext _context = new WorkflowDefinition.IOMappingContext();

        private WorkflowActivity _first;       

        public WorkflowActivityBuilder<TActivity> StartWith<TActivity>(Action<TActivity> configure = null)
            where TActivity : WorkflowActivity, new()
        {
            var firstActivity = new TActivity();
            configure?.Invoke(firstActivity);

            _flow.AddVertex(firstActivity);
            _first = firstActivity;

            return new WorkflowActivityBuilder<TActivity>(this, firstActivity);
        }

        public WorkflowDefinition Build() => new WorkflowDefinition(_flow, _first, _context);
    }
}
