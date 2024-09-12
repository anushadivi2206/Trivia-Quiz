using Combination.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Combination.Services
{
    public class LeaderboardService
    {
        private const int MaxLeaderboardEntries = 5;
        private List<LeaderboardEntry> _leaderboard = new List<LeaderboardEntry>();

        public void AddScore(string playerName, int score)
        {
            _leaderboard.Add(new LeaderboardEntry { PlayerName = playerName, Score = score, Date = DateTime.Now });
            _leaderboard = _leaderboard.OrderByDescending(e => e.Score).Take(MaxLeaderboardEntries).ToList();
        }

        public List<LeaderboardEntry> GetLeaderboard()
        {
            return _leaderboard.ToList();
        }
    }

    public class LeaderboardEntry
    {
        public string PlayerName { get; set; }
        public int Score { get; set; }
        public DateTime Date { get; set; }
    }
}