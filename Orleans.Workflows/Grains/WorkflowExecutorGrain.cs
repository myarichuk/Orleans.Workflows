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

        public Task<ActivityContext> ExecuteSingleAsync(WorkflowActivity activity, ActivityContext context)
        {
            //TODO: consider different grain allocation strategy
            //perhaps it should be created from hashcode of activity implementation or something
            var worker = GrainFactory.GetGrain<IWorkerGrain>(Guid.NewGuid());
            try
            {
                return worker.ExecuteAsync(activity,context);
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"Unhandled exception thrown while running activity of type = {activity.GetType().FullName}");
            }

            return Task.FromResult<ActivityContext>(null);
        }

        public Task<ActivityContext> ExecuteAsync(WorkflowDefinition workflow)
        {
            return Task.FromResult((ActivityContext)null);
        }
    }
}
