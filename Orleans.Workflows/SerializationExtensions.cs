using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq.Expressions;
using Force.DeepCloner;
using Humanizer.Bytes;
using Orleans.CodeGeneration;
using Orleans.Serialization;
using Serialize.Linq.Serializers;
using Utf8Json;
using JsonSerializer = Utf8Json.JsonSerializer;

namespace Orleans.Workflows.Workflow
{
    [Serializer(typeof(EdgeWithPredicate))]
    public static class EdgeWithPredicateSerializer
    {
        private static readonly ExpressionSerializer ExpressionSerializer = new ExpressionSerializer(new Serialize.Linq.Serializers.JsonSerializer());

        [CopierMethod]
        public static object DeepCopier(object original, ICopyContext context)
        {
            var clone = original.DeepClone();
            context.RecordCopy(original, clone);

            return clone;
        }

        [SerializerMethod]
        public static void Serializer(object untypedInput, ISerializationContext context, Type expected)
        {
            var input = (EdgeWithPredicate)untypedInput;

            SerializationManager.SerializeInner(input.Source, context);
            SerializationManager.SerializeInner(input.Target, context);
            SerializationManager.SerializeInner(ExpressionSerializer.SerializeBinary(input.Predicate), context);
        }

        [DeserializerMethod]
        public static object Deserializer(Type expected, IDeserializationContext context)
        {
            var source = SerializationManager.DeserializeInner<WorkflowActivity>(context);
            var target = SerializationManager.DeserializeInner<WorkflowActivity>(context);
            var predicate = (Expression<Func<ActivityContext, bool>>)ExpressionSerializer.DeserializeBinary(SerializationManager.DeserializeInner<byte[]>(context));

            return new EdgeWithPredicate
            {
                Source = source,
                Target = target,
                Predicate = predicate
            };
        }
    }

    [Serializer(typeof(WorkflowActivity))]
    public static class ActivityWorkflowSerializer
    {
        private static readonly ConcurrentDictionary<string, Type> ActivityTypeCache = new ConcurrentDictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

        [CopierMethod]
        public static object DeepCopier(object original, ICopyContext context)
        {
            var clone = original.DeepClone();
            context.RecordCopy(original, clone);

            return clone;
        }

        [SerializerMethod]
        public static void Serializer(object untypedInput, ISerializationContext context, Type expected)
        {
            var type = untypedInput.GetType();

            var serialized = JsonSerializer.Serialize(untypedInput);

            //TODO: differentiate between activities defined in Orleans.Workflows and activities defined in other assemblies (no need to compile/decompile)
            SerializationManager.SerializeInner(type.ToSourceCode(), context);
            SerializationManager.SerializeInner(serialized, context);
        }

        [DeserializerMethod]
        public static object Deserializer(Type expected, IDeserializationContext context)
        {
            var typeSourceCode = SerializationManager.DeserializeInner<string>(context);
            
            var workflowType = ActivityTypeCache.GetOrAdd(typeSourceCode, src => src.CompileFromSourceCode());

            var objectBytes = SerializationManager.DeserializeInner<byte[]>(context);

            var workflowActivity = JsonSerializer.NonGeneric.Deserialize(workflowType, objectBytes);
            context.RecordObject(workflowActivity);

            return workflowActivity;
        }
    }

    [Serializer(typeof(ActivityContext))]
    public static class ExpandoObjectSerializer
    {
        [CopierMethod]
        public static object DeepCopier(object original, ICopyContext context)
        {
            var input = (IDictionary<string, object>) original;
            var result = new ActivityContext();

            foreach (var kvp in input)
                result.Data.Add(kvp.Key, kvp.Value);

            context.RecordCopy(original, result);

            return result;
        }

        [SerializerMethod]
        public static void Serializer(object untypedInput, ISerializationContext context, Type expected)
        {
            var input = ((ActivityContext)untypedInput).Data;

            SerializationManager.SerializeInner(input, context);
        }

        [DeserializerMethod]
        public static object Deserializer(Type expected, IDeserializationContext context)
        {
            var result = new ActivityContext();

            // Record 'result' immediately after constructing it. As with with the deep copier, this
            // allows for cyclic references and de-duplication.
            context.RecordObject(result);

            var deserialized = SerializationManager.DeserializeInner<IDictionary<string, object>>(context);
            foreach (var kvp in deserialized)
                result.Data.Add(kvp.Key, kvp.Value);

            return result;
        }
    }
}
