using Microsoft.AspNetCore.SignalR;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Combination.Hubs
{
    public class QuizHub : Hub
    {
        private static Dictionary<string, string> roomAdmins = new Dictionary<string, string>();


        public async Task JoinRoom(string roomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            if (!roomAdmins.ContainsKey(roomId))
            {
                roomAdmins[roomId] = Context.ConnectionId;
            }
        }
        public async Task BroadcastFinalScores(string gamePin, Dictionary<string, int> finalScores)
        {
            await Clients.Group(gamePin).SendAsync("ReceiveFinalScores", finalScores);
        }

        public async Task SendScoreUpdate(string connectionId, int score)
        {
            await Clients.Client(connectionId).SendAsync("UpdateScore", score);
        }

        public async Task BroadcastQuestionNavigation(string gamePin, int questionIndex)
        {
            await Clients.Group(gamePin).SendAsync("NavigateToQuestion", questionIndex);
        }



        public string GetAdminConnectionId(string roomId)
        {
            return roomAdmins.TryGetValue(roomId, out var adminId) ? adminId : null;
        }

        public async Task UpdateCategories(string roomId, HashSet<string> categories)
        {
            await Clients.Group(roomId).SendAsync("UpdateCategories", categories);
        }

        public async Task StartQuiz(string roomId)
        {
            await Clients.Group(roomId).SendAsync("StartQuiz");
        }

        public async Task SendAnswerResult(string userId, bool isCorrect, string message)
        {
            await Clients.Client(userId).SendAsync("ReceiveAnswerResult", isCorrect, message);
        }
    }
}