using System.Collections.Generic;

namespace KChannelAdvisor.Descriptor.API.Mapper
{
    internal class KCMappingContext
    {
        private IDictionary<string, object> Variables;

        public KCMappingContext()
        {
            Variables = new Dictionary<string, object>();
        }

        public object GetVariable(string name)
        {
            return Variables[name];
        }

        public void SetVariable(string name, object value)
        {
            if (Variables.ContainsKey(name))
            {
                Variables[name] = value;
            }
            else
            {
                Variables.Add(name, value);
            }
        }

        public void SetVariables(IDictionary<string, object> variables)
        {
            Variables = variables;
        }
    }
}
