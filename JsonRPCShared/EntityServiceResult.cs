using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonRPCShared
{
    public class EntityServiceResult
    {
        public EntityServiceResult(object result)
        {
            this.Result = result;
        }

        public object Result { get; private set; }
    }
}
