using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Combination.Services;

namespace Combination.Hubs
{
    public class RoomHub : Hub
    {
        private readonly RoomService _roomService;

        public RoomHub(RoomService roomService)
        {
            _roomService = roomService;
        }

        public async Task JoinRoom(string gamePin)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, gamePin);
        }

        public async Task StartQuiz(string gamePin, string categories)
        {
            await Clients.Group(gamePin).SendAsync("NavigateToQuiz", categories, gamePin);
        }

        public async Task LeaveRoom(string gamePin, string playerName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, gamePin);
            var room = _roomService.GetRoom(gamePin);
            if (room != null)
            {
                string playerIdToRemove = room.Members.FirstOrDefault(x => x.Value == playerName).Key;
                if (playerIdToRemove != null)
                {
                    if (playerIdToRemove == room.CreatorId)
                    {
                        // Creator is leaving, delete the room
                        _roomService.DeleteRoom(gamePin);
                        await Clients.Group(gamePin).SendAsync("RoomDeleted");
                    }
                    else
                    {
                        // Regular member leaving
                        _roomService.RemoveMember(gamePin, playerName);

                        // Update the room object after removing the member
                        room = _roomService.GetRoom(gamePin);

                        // Send updated members list to all clients in the room
                        await SendMembersUpdate(gamePin, room.Members);
                    }
                }
            }
        }

        public async Task UpdateCategories(string gamePin, HashSet<string> categories)
        {
            await Clients.Group(gamePin).SendAsync("ReceiveCategoryUpdate", categories);
        }



        public async Task SendMembersUpdate(string gamePin, Dictionary<string, string> members)
        {
            await Clients.Group(gamePin).SendAsync("ReceiveMembersUpdate", members);
        }

        public async Task StartGame(string gamePin)
        {
            await Clients.Group(gamePin).SendAsync("StartGame");
        }

        public async Task RoomDeleted(string gamePin)
        {
            await Clients.Group(gamePin).SendAsync("RoomDeleted");
        }
    }
}