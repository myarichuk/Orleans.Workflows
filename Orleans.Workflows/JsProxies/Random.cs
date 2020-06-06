using System;
using Jurassic;
using Jurassic.Library;

namespace Orleans.Workflows.JsProxies
{
    public class RandomConstructor : ClrFunction
    {
        public RandomConstructor(ScriptEngine engine)
            : base(engine.Function.InstancePrototype, "Random", new RandomInstance(engine.Object.InstancePrototype))
        {
        }

        [JSConstructorFunction]
        public RandomInstance Construct(int seed) => new RandomInstance(InstancePrototype, seed);
    }

    public class RandomInstance : ObjectInstance
    {
        private readonly Random _random;

        public RandomInstance(ObjectInstance prototype)
            : base(prototype)
        {
            PopulateFunctions();
            _random = new Random(0);
        }

        public RandomInstance(ObjectInstance prototype, int seed)
            : base(prototype)
        {
            _random = new Random(seed);
        }

        [JSFunction(Name = "nextInt")]
        public int NextInt() => _random.Next();

        [JSFunction(Name = "nextDouble")]
        public double NextDouble() => _random.NextDouble();
    }}
