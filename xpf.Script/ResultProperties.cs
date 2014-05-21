using System;
using System.Collections.Generic;
using System.Dynamic;

namespace xpf.Scripting
{
    public class ResultProperties : DynamicObject
    {
        readonly Dictionary<string, object> _values;
        // Used to inform user what fields are available when they get it wrong!
        string formattedFieldList = null;

        public ResultProperties(Dictionary<string, object> values)
        {
            this._values = values;

            // Record formatted list of fields
            var fieldList = new List<string>();
            foreach(var k in this._values.Keys)
                fieldList.Add(k);

            this.formattedFieldList = string.Join(", ", fieldList);
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (this._values.ContainsKey(binder.Name.ToLower()))
            {
                result = this._values[binder.Name.ToLower()];
                return true;
            }
            else
            {
                throw new ArgumentException(string.Format("The property {0} is not part of the result. The result has the following properties: {1}", binder.Name, this.formattedFieldList));
            }
        }
    }
}