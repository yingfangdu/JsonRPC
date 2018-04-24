using System;
using System.Reflection;
using JsonRPCShared;
using Newtonsoft.Json.Linq;
using JsonRPCService;

namespace JsonRPCServer
{
    public class JsonService
    {
        public EntityServiceResult CallEntityService(JToken parameter)
        {
            EntityServiceRequest request = parameter.ToObject<EntityServiceRequest>();

            object service = EntityServiceManager.Instance.GetService(request.Name);

            Type type = service.GetType();
            MethodInfo theMethod = type.GetMethod(request.MethodName);

            object returnValue = null;
            if (theMethod != null)
            {
                returnValue = theMethod.Invoke(service, request.Parameters);
            }

            return new EntityServiceResult(returnValue);
        }
    }
}
