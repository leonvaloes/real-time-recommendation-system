using CleanArch.Domain.Entities;
using CleanArch.Domain.ValueObjects;
using CleanArch.Infra.Data.Interface;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;

namespace CleanArch.Infra.Data.Context
{
    public class MongoContext : IMongoContext
    {
        private static bool _serializersConfigured;
        private readonly MongoClient _client;
        private readonly IMongoDatabase _database;

        public IMongoCollection<User> Users { get; private set; } = null!;

        public IMongoDatabase GetDatabase(string databaseName)
        {
            return _client.GetDatabase(databaseName);
        }

        public MongoContext()
        {
            ConfigureSerializers();

            var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");
            var databaseName = Environment.GetEnvironmentVariable("MONGODB_DB");

            if (connectionString == null)
            {
                Console.WriteLine("You must set your 'MONGODB_URI' environment variable.");
                Environment.Exit(0);
            }

            if (databaseName == null)
            {
                Console.WriteLine("You must set your 'MONGODB_DB' environment variable.");
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

        private static void ConfigureSerializers()
        {
            if (_serializersConfigured)
            {
                return;
            }

            BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));
            ConfigureUserMap();
            ConfigureNaturalPersonMap();
            ConfigureJuridicalPersonMap();
            ConfigureCpfMap();
            ConfigureCnpjMap();
            ConfigureRoleMap();
            ConfigurePermissionClaimMap();

            _serializersConfigured = true;
        }

        private static void ConfigureUserMap()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(User)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<User>(classMap =>
            {
                classMap.MapIdMember(user => user.Id)
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                classMap.MapMember(user => user.Email);
                classMap.MapMember(user => user.PasswordHash);
                classMap.MapMember(user => user.Phone).SetIgnoreIfNull(true);
                classMap.MapMember(user => user.PersonType);
                classMap.MapMember(user => user.NaturalPerson).SetIgnoreIfNull(true);
                classMap.MapMember(user => user.JuridicalPerson).SetIgnoreIfNull(true);
                classMap.MapMember(user => user.IsActive);
                classMap.MapMember(user => user.CreatedAt);
                classMap.MapMember(user => user.UpdatedAt);
                classMap.MapMember(user => user.LastActivity).SetIgnoreIfNull(true);
                classMap.MapMember(user => user.Roles);
                classMap.SetIgnoreExtraElements(true);
            });
        }

        private static void ConfigureNaturalPersonMap()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(NaturalPerson)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<NaturalPerson>(classMap =>
            {
                classMap.MapMember(person => person.Id)
                    .SetElementName("id")
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                classMap.MapMember(person => person.FirstName);
                classMap.MapMember(person => person.LastName);
                classMap.MapMember(person => person.CPF).SetIgnoreIfNull(true);
                classMap.SetIgnoreExtraElements(true);
            });
        }

        private static void ConfigureJuridicalPersonMap()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(JuridicalPerson)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<JuridicalPerson>(classMap =>
            {
                classMap.MapMember(person => person.Id)
                    .SetElementName("id")
                    .SetSerializer(new GuidSerializer(GuidRepresentation.Standard));
                classMap.MapMember(person => person.CNPJ);
                classMap.MapMember(person => person.RegisteredName);
                classMap.MapMember(person => person.BusinessName).SetIgnoreIfNull(true);
                classMap.SetIgnoreExtraElements(true);
            });
        }

        private static void ConfigureCpfMap()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Cpf)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<Cpf>(classMap =>
            {
                classMap.MapMember(cpf => cpf.Value);
                classMap.SetIgnoreExtraElements(true);
            });
        }

        private static void ConfigureCnpjMap()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Cnpj)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<Cnpj>(classMap =>
            {
                classMap.MapMember(cnpj => cnpj.Value);
                classMap.SetIgnoreExtraElements(true);
            });
        }

        private static void ConfigureRoleMap()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(Role)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<Role>(classMap =>
            {
                classMap.MapMember(role => role.Name);
                classMap.MapMember(role => role.Claims);
                classMap.SetIgnoreExtraElements(true);
            });
        }

        private static void ConfigurePermissionClaimMap()
        {
            if (BsonClassMap.IsClassMapRegistered(typeof(PermissionClaim)))
            {
                return;
            }

            BsonClassMap.RegisterClassMap<PermissionClaim>(classMap =>
            {
                classMap.MapMember(claim => claim.Type);
                classMap.MapMember(claim => claim.Value);
                classMap.SetIgnoreExtraElements(true);
            });
        }
    }
}
