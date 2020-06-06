using System;
using System.Collections.Generic;
using System.Dynamic;
using Orleans.CodeGeneration;
using Orleans.Serialization;

namespace Orleans.Workflows.Workflow
{
    [CodeGeneration.SerializerAttribute(typeof(ExpandoObject))]
    public class ExpandoObjectSerializer
    {
        [CopierMethod]
        public static object DeepCopier(object original, ICopyContext context)
        {
            var input = (IDictionary<string, object>) original;
            dynamic result = new ExpandoObject();

            foreach (var kvp in input)
                ((IDictionary<string, object>) result).Add(kvp);

            context.RecordCopy(original, result);

            return result;
        }

        [SerializerMethod]
        public static void Serializer(object untypedInput, ISerializationContext context, Type expected)
        {
            var input = (IDictionary<string, object>) untypedInput;

            SerializationManager.SerializeInner(input, context);
        }

        [DeserializerMethod]
        public static object Deserializer(Type expected, IDeserializationContext context)
        {
            dynamic result = new ExpandoObject();

            // Record 'result' immediately after constructing it. As with with the deep copier, this
            // allows for cyclic references and de-duplication.
            context.RecordObject(result);

            var deserialized = SerializationManager.DeserializeInner<IDictionary<string, object>>(context);
            foreach (var kvp in deserialized)
                ((IDictionary<string, object>) result).Add(kvp);

            return result;
        }
    }
}
