using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text;

namespace Orleans.Workflows
{
    //TODO: add implicit operator to convert to and from ExpandoObject and Dictionary<string, object> (for convenience) 
    [Serializable]
    public class ActivityContext : DynamicObject
    {
        internal Dictionary<string, object> Data = new Dictionary<string, object>(StringComparer.InvariantCultureIgnoreCase);

        public int Count => Data.Count;

        public override bool TryGetMember(
            GetMemberBinder binder, out object result) => Data.TryGetValue(binder.Name, out result);

        public override bool TrySetMember(
            SetMemberBinder binder, object value)
        {
            Data[binder.Name] = value;

            return true;
        }
    }
}
