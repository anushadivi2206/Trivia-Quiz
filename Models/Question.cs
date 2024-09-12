// Models/Question.cs
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Combination.Models
{
    public class Question1
    {
        [BsonId]
        public ObjectId _id { get; set; } = ObjectId.GenerateNewId();

        [BsonElement("category")]
        public string Category { get; set; } = string.Empty;

        [BsonElement("subcategory")]
        public string Subcategory { get; set; } = string.Empty;

        [BsonElement("text")]
        public string Text { get; set; } = string.Empty;

        [BsonElement("options")]
        public List<string> Options { get; set; } = new();

        [BsonElement("correctAnswer")]
        public string CorrectAnswer { get; set; } = string.Empty;
        public List<string> Answers => Options;
        public int TimeLeft { get; set; }
        public bool IsAnswered { get; set; }

        public bool AnsweredCorrectly { get; set; } = false;
        public bool IsSubmitted { get; set; }
        public int Score { get; set; }
        public bool ShowTimesUpMessage { get; set; }
        public string SelectedAnswer { get; set; }

        public List<string> SelectedAnswers { get; set; } = new List<string>();
        public List<string> CorrectAnswers { get; set; } = new List<string>();
        public bool IsCorrect { get; set; }

        public bool AnswerSelected { get; set; } = false;
        public string AnsweringPlayer { get; set; }
        public Dictionary<string, string> PlayerAnswers { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, bool> PlayerCorrectness { get; set; } = new Dictionary<string, bool>();

        public Dictionary<string, int> PlayerScores { get; set; } = new Dictionary<string, int>();

        public bool IsCorrectAnswer(string userAnswer)
        {
            return string.Equals(userAnswer, CorrectAnswer, System.StringComparison.OrdinalIgnoreCase);
        }

    }
}