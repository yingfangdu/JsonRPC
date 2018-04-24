namespace JsonRPCService
{
    using System;
    using System.Collections.Concurrent;

    public class EntityServiceManager
    {
        private ConcurrentDictionary<string, EntityService> entityServicesCache = new ConcurrentDictionary<string, EntityService>();

        private static EntityServiceManager s_serviceManager = new EntityServiceManager();

        public static EntityServiceManager Instance
        {
            get { return s_serviceManager; }
        }

        public EntityService GetService(string name)
        {
            string fullName = "JsonRPCService." + name;

            return this.entityServicesCache.GetOrAdd(fullName, (key) => Activator.CreateInstance(Type.GetType(key)) as EntityService);
        }
    }
}
