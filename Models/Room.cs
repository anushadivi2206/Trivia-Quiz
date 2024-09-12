using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Combination.Models
{
    public class Room
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string GamePin { get; set; }
        public string CreatorId { get; set; }
        public Dictionary<string, string> Members { get; set; } = new Dictionary<string, string>();

        public bool IsFull => Members.Count >= 5;
        public string ConnectionString { get; set; }
        public string QuizAppDatabaseName { get; set; }
        public string WiseWorkDatabaseName { get; set; }

        public string AddMember(string memberId, string playerName)
        {
            if (IsFull) return null;
            Members[memberId] = playerName;
            return playerName;
        }

        public void UpdateMembers(Dictionary<string, string> newMembers)
        {
            Members.Clear();
            foreach (var member in newMembers)
            {
                Members.Add(member.Key, member.Value);
            }
        }
    }
}