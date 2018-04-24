namespace JsonRPCService
{
    public class EntityService
    {
        public int GetCount()
        {
            return 100;
        }

        public object[] GetEntities(long startIndex, long pageSize, long sortOption, string sortColumn)
        {
            return new string[] { "Test1", "Test2", "Test3" };
        }
    }

    public class CampaignService : EntityService
    {

    }
}
