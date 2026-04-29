using MongoDB.Driver;

namespace UserAuth.CleanArch.Infra.Data.Interface
{
    public interface IMongoContext
    {
        IMongoDatabase GetDatabase(string databaseName);
    }
}
