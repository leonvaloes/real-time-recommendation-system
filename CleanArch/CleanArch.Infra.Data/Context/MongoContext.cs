using CleanArch.Domain.Entities;
using CleanArch.Infra.Data.Interface;
using MongoDB.Driver;


namespace CleanArch.Infra.Data.Context
{
    public class MongoContext : IMongoContext
    {
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public IMongoCollection<User> Users { get; private set; }

        public IMongoDatabase GetDatabase(string databaseName)
        {
            return _client.GetDatabase(databaseName);
        }

        public MongoContext()
        {
            var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
            var databaseName = Environment.GetEnvironmentVariable("MONGODB_DB");

            if (connectionString == null)
            {
                Console.WriteLine("You must set your 'MONGODB_URI' environment variable. To learn how to set it, see https://www.mongodb.com/docs/drivers/csharp/current/get-started/create-connection-string");
                Environment.Exit(0);
            }
            if (databaseName == null)
            {
                Console.WriteLine("You must set your 'MONGODB_DB' environment variable. To learn how to set it, see XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX");
                Environment.Exit(0);
            }

            _client = new MongoClient(connectionString);
            _database = _client.GetDatabase(databaseName);
            Console.WriteLine("Connected to MongoDB");

            InitializeCollections();
        }
        private void InitializeCollections()
        {
            Console.WriteLine("Initializing collections");
            Users = _database.GetCollection<User>("users");
        }
    }
}
