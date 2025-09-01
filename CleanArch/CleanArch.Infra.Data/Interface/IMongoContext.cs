using MongoDB.Driver;

namespace CleanArch.Infra.Data.Interface
{
    public interface IMongoContext
    {
        IMongoDatabase GetDatabase(string databaseName);
    }
}
