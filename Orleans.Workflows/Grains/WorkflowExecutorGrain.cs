using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Workflows.Interfaces;

namespace Orleans.Workflows.Grains
{
    [Serializable]
    public class WorkflowState
    {
        public ActivityContext ExecutionContext { get; set; }

        public EdgeWithPredicate Current { get; set; }

        public HashSet<Guid> Visited { get; set; }
    }

    public class WorkflowExecutorGrain : Grain, IWorkflowExecutorGrain
    {
        private readonly IPersistentState<WorkflowState> _flowState;
        private readonly ILogger<WorkflowExecutorGrain> _logger;

        public WorkflowExecutorGrain(
            [PersistentState(nameof(_flowState))] IPersistentState<WorkflowState> flowState,
            ILogger<WorkflowExecutorGrain> logger)
        {
            _flowState = flowState;
            _logger = logger;
        }

        public Task<ActivityContext> ExecuteAsync(WorkflowDefinition workflow)
        {
            return Task.FromResult((ActivityContext)null);
        }
    }
}
