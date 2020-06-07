using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Force.DeepCloner;
using Orleans.CodeGeneration;
using Orleans.Serialization;
using Utf8Json;

namespace Orleans.Workflows.Workflow
{
    [Serializer(typeof(WorkflowActivity))]
    public static class ActivityWorkflowSerializer
    {
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

            SerializationManager.SerializeInner(type.ToSourceCode(), context);
            SerializationManager.SerializeInner(serialized, context);
        }

        private static readonly ConcurrentDictionary<string, Type> _workflowTypes = new ConcurrentDictionary<string, Type>(StringComparer.InvariantCultureIgnoreCase);

        [DeserializerMethod]
        public static object Deserializer(Type expected, IDeserializationContext context)
        {
            var typeSourceCode = SerializationManager.DeserializeInner<string>(context);

            var workflowType = _workflowTypes.GetOrAdd(typeSourceCode, src => src.CompileFromSourceCode());

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
