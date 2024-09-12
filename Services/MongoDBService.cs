using MongoDB.Driver;
using Microsoft.Extensions.Configuration;

namespace Combination.Services
{
    public class MongoDBService
    {
        private readonly IMongoDatabase _database1;
        private readonly IMongoDatabase _database2;

        public MongoDBService(IConfiguration config)
        {
            var connectionString1 = config.GetSection("MongoDB1:ConnectionString").Value;
            var databaseName1 = config.GetSection("MongoDB1:DatabaseName").Value;

            var connectionString2 = config.GetSection("MongoDB2:ConnectionString").Value;
            var databaseName2 = config.GetSection("MongoDB2:DatabaseName").Value;

            if (string.IsNullOrEmpty(connectionString1) || string.IsNullOrEmpty(connectionString2))
            {
                throw new ArgumentNullException("ConnectionString", "MongoDB connection string is not configured properly.");
            }

            var client1 = new MongoClient(connectionString1);
            _database1 = client1.GetDatabase(databaseName1);

            var client2 = new MongoClient(connectionString2);
            _database2 = client2.GetDatabase(databaseName2);
        }

        public IMongoCollection<T> GetCollection1<T>(string name)
        {
            return _database1.GetCollection<T>(name);
        }

        public IMongoCollection<T> GetCollection2<T>(string name)
        {
            return _database2.GetCollection<T>(name);
        }
    }
}
