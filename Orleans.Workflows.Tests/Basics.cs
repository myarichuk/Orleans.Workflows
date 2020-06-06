using System;
using System.Dynamic;
using System.Threading.Tasks;
using Orleans.TestingHost;
using Xunit;

namespace Orleans.Workflows.Tests
{
    [Collection(ClusterCollection.Name)]
    public class Basics
    {
        private readonly TestCluster _cluster;

        public Basics(ClusterFixture fixture) => _cluster = fixture.Cluster;

        [Fact]
        public void Can_instantiate()
        {
            var wb = new WorkflowBuilder();
            var workflow = 
                wb.StartWith<AddTwoNumbers>()
                  .Build();
            
            var source = typeof(AddTwoNumbers).ToSourceCode();
        }

        [Serializable]
        public class AddTwoNumbers : WorkflowActivity
        {
            public int X { get; set; }

            public int Y { get; set; }

            public override Task<ExpandoObject> Execute(ExpandoObject context)
            {
                dynamic result = new ExpandoObject();
                result.Result = X + Y;
                return Task.FromResult(result);
            }
        }
    }
}
