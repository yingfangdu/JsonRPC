using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonRPCShared
{
    public class EntityServiceRequest
    {
        public EntityServiceRequest(string name, string methodName, object[] parameters)
        {
            this.Name = name;
            this.MethodName = methodName;
            this.Parameters = parameters;
        }

        public string Name { get; set; }

        public string MethodName { get; set; }

        public object[] Parameters { get; set; }
    }
}
