using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Orleans.Runtime;
using Orleans.Workflows.Interfaces;
using Orleans.Workflows.Workflow;

namespace Orleans.Workflows.Grains
{
    [Serializable]
    public class WorkflowState
    {
        public ExpandoObject ExecutionContext { get; set; }

        public EdgeWithPredicate Current { get; set; }
    }

    public class OrchestratorGrain : Grain, IOrchestratorGrain
    {
        private readonly IPersistentState<WorkflowState> _flowState;
        private readonly ILogger<OrchestratorGrain> _logger;

        public OrchestratorGrain([PersistentState(nameof(_flowState))] IPersistentState<WorkflowState> flowState, ILogger<OrchestratorGrain> logger)
        {
            _flowState = flowState;
            _logger = logger;
        }

        public Task Execute(WorkflowDefinition workflow)
        {
            return Task.CompletedTask;
        }
    }
}
