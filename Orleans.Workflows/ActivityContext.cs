using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Orleans.Workflows
{
    //TODO: add implicit operator to convert to and from ExpandoObject and Dictionary<string, object> (for convenience) 
    [Serializable]
    public class ActivityContext : DynamicObject
    {
        internal Dictionary<string, object> Data = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        public int Count => Data.Count;

        public override bool TryGetMember(GetMemberBinder binder, out object result) => Data.TryGetValue(binder.Name, out result);

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            Data[binder.Name] = value;
            return true;
        }

        public override IEnumerable<string> GetDynamicMemberNames() => Data.Keys;

        public static implicit operator Dictionary<string, object>(ActivityContext context) => context.Data;

        public static implicit operator ActivityContext(ExpandoObject eo) =>
            new ActivityContext
            {
                Data = new Dictionary<string, object>(eo)
            };

        public static implicit operator ExpandoObject(ActivityContext context)
        {
            var eo = new ExpandoObject();

            foreach(var kvp in context.Data)
                ((IDictionary<string, object>)eo).Add(kvp);

            return eo;
        }
    }
}
