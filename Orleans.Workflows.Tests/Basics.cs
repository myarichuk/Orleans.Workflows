using System;
using System.Dynamic;
using System.Runtime.InteropServices.ComTypes;
using System.Threading.Tasks;
using Orleans.TestingHost;
using Orleans.Workflows.Interfaces;
using Xunit;

namespace Orleans.Workflows.Tests
{
    public class AddTwoNumbers : WorkflowActivity
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Result { get; set; }

        public override Task ExecuteAsync(ActivityContext context)
        {
            Result = X + Y;
            return Task.CompletedTask;
        }
    }

    public class MultiplyTwoNumbers : WorkflowActivity
    {
        public int X { get; set; }
        public int Y { get; set; }

        public int Result { get; set; }

        public override Task ExecuteAsync(ActivityContext context)
        {
            Result = X * Y;
            return Task.CompletedTask;
        }
    }

    [Collection(ClusterCollection.Name)]
    public class Basics
    {
        private readonly TestCluster _cluster;

        public Basics(ClusterFixture fixture) => _cluster = fixture.Cluster;

        //[Fact]
        //public async Task Can_execute_compiled_activity()
        //{
        //    var source = typeof(AddTwoNumbers).ToSourceCode();

        //    var compiledType = source.CompileFromSourceCode();
        //    var activity = (WorkflowActivity)Activator.CreateInstance(compiledType);

        //    dynamic context = new ActivityContext();

        //    context.X = 5;
        //    context.Y = 12;

        //    await activity.ExecuteAsync(context);

        //    Assert.Equal(17, ((dynamic)activity).Result);
        //}


        [Fact]
        public async Task Can_execute_activity_sequence()
        {
            var workflowDefinition = new WorkflowBuilder()
                .StartWith<AddTwoNumbers>()
                    .Input(activity => activity.X, 12)
                    .Input(activity => activity.Y, 24)
                    .Output(activity => activity.Result, ctx => ctx["Result"])
                .Then<MultiplyTwoNumbers>()
                    .Input(activity => activity.X, 3)
                    .Input(activity => activity.Y, ctx => ctx["Result"])
                .Build();
        }
    }
}
