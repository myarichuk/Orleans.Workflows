using System;
using System.Dynamic;
using QuickGraph;

namespace Orleans.Workflows
{
    public class EdgeWithPredicate : IEdge<WorkflowActivity>
    {
        public WorkflowActivity Source { get; set; }
        public WorkflowActivity Target { get; set; }

        public Func<ExpandoObject, bool> Predicate { get; set; } = _ => true;
    }

    public class WorkflowDefinition
    {
        private readonly AdjacencyGraph<WorkflowActivity, EdgeWithPredicate> _flow;

        public WorkflowActivity FirstActivity { get; }
        public IGraph<WorkflowActivity, EdgeWithPredicate> Flow => _flow;

        public WorkflowDefinition(AdjacencyGraph<WorkflowActivity, EdgeWithPredicate> flow, WorkflowActivity first)
        {
            _flow = flow;
            FirstActivity = first;
        }
    }

    public class WorkflowBuilder
    {
        private readonly AdjacencyGraph<WorkflowActivity, EdgeWithPredicate> _flow = 
            new AdjacencyGraph<WorkflowActivity, EdgeWithPredicate>(true);

        private WorkflowActivity _first;
        
        public WorkflowBuilder StartWith<TActivity>()
            where TActivity : WorkflowActivity, new()
        {
            var firstActivity = new TActivity();
            _flow.AddVertex(firstActivity);
            _first = firstActivity;

            return this;
        }

        public WorkflowDefinition Build() => new WorkflowDefinition(_flow, _first);
    }
}
