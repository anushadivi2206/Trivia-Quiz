using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Combination.Models;
using Combination.Services;

namespace Combination.Services
{
    public class RoomService
    {
        private readonly IMongoCollection<Room> _rooms;
        private const string AllowedCharacters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        private const int PinLength = 6;

        public RoomService(MongoDBService mongoDBService)
        {
            _rooms = mongoDBService.GetCollection2<Room>("Rooms");
        }



        public (Room, string) CreateRoom(string playerId, string playerName)
        {
            var room = new Room
            {
                GamePin = GenerateGamePin(),
                Members = new Dictionary<string, string> { { playerId, playerName } },
                CreatorId = playerId
            };
            _rooms.InsertOne(room);
            return (room, playerName);
        }

        public (bool, string) JoinRoom(string gamePin, string playerId, string playerName)
        {
            var room = _rooms.Find(r => r.GamePin == gamePin).FirstOrDefault();
            if (room == null)
                return (false, null);
            room.Members[playerId] = playerName;
            _rooms.ReplaceOne(r => r.GamePin == gamePin, room);
            return (true, playerName);
        }

        public Room GetRoom(string gamePin)
        {
            return _rooms.Find(r => r.GamePin == gamePin).FirstOrDefault();
        }

        public void DeleteRoom(string gamePin)
        {
            _rooms.DeleteOne(r => r.GamePin == gamePin);
        }

        public void RemoveMember(string gamePin, string playerName)
        {
            var room = _rooms.Find(r => r.GamePin == gamePin).FirstOrDefault();
            if (room != null)
            {
                var memberIdToRemove = room.Members.FirstOrDefault(m => m.Value == playerName).Key;
                if (memberIdToRemove != null)
                {
                    room.Members.Remove(memberIdToRemove);
                    _rooms.ReplaceOne(r => r.GamePin == gamePin, room);
                }
            }
        }

        private string GenerateGamePin()
        {
            var random = new Random();
            var pinBuilder = new StringBuilder(PinLength);
            for (int i = 0; i < PinLength; i++)
            {
                pinBuilder.Append(AllowedCharacters[random.Next(AllowedCharacters.Length)]);
            }
            return pinBuilder.ToString();
        }
    }
}