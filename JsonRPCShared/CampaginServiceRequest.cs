using StreamJsonRpc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonRPCShared
{
    public class CampaginServiceGetCountRequest
    {
        public static async Task<EntityServiceResult> MakeCall(JsonRpc jsonRpc)
        {
            return await jsonRpc.InvokeWithParameterObjectAsync<EntityServiceResult>("CallEntityService", new EntityServiceRequest("CampaignService", "GetCount", null));
        }
    }

    public class CampaginServiceGetEntityRequest
    {
        public static async Task<EntityServiceResult> MakeCall(JsonRpc jsonRpc, int startIndex, int pageSize, int sortOption, string sortColumnName)
        {
            return await jsonRpc.InvokeWithParameterObjectAsync<EntityServiceResult>("CallEntityService", new EntityServiceRequest("CampaignService", "GetEntities", new object[] { startIndex, pageSize, sortOption, sortColumnName }));
        }
    }
}
