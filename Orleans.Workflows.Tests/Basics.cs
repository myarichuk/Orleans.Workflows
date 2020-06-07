using System;
using System.Dynamic;
using System.Threading.Tasks;
using Orleans.TestingHost;
using Orleans.Workflows.Interfaces;
using Xunit;

namespace Orleans.Workflows.Tests
{
    [Serializable]
    public class AddTwoNumbers : WorkflowActivity
    {
        public override Task<ActivityContext> ExecuteAsync(ActivityContext context)
        {
            var input = (dynamic)context;

            input.Result = input.X + input.Y;
            return Task.FromResult(context);
        }
    }

    [Collection(ClusterCollection.Name)]
    public class Basics
    {
        private readonly TestCluster _cluster;

        public Basics(ClusterFixture fixture) => _cluster = fixture.Cluster;

        [Fact]
        public async Task Can_execute_compiled_activity()
        {           
            var source = typeof(AddTwoNumbers).ToSourceCode();

            var compiledType = source.CompileFromSourceCode();
            var worker = (WorkflowActivity)Activator.CreateInstance(compiledType);

            dynamic context = new ActivityContext();
            
            context.X = 5;
            context.Y = 12;

            var response = await worker.ExecuteAsync(context);

            Assert.Equal(17, response.Result);
        }

        [Fact]
        public async Task Can_execute_remote_activity()
        {
            var orchestrator = _cluster.Client.GetGrain<IWorkflowExecutorGrain>(Guid.NewGuid());
            var activity = new AddTwoNumbers();

            dynamic context = new ExpandoObject();

            context.X = 5;
            context.Y = 12;

            var activityResult = await orchestrator.ExecuteSingle(activity, context);
            Assert.Equal(17, activityResult.Result);
        }
    }
}
