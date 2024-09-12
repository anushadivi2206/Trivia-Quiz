using MongoDB.Driver;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Combination.Models;


namespace Combination.Services
{
    public class QuestionService
    {
        private readonly IMongoCollection<Question1> _questions;
        public IConfiguration _configuration;
        private readonly Dictionary<string, List<Question1>> _roomQuestions = new Dictionary<string, List<Question1>>();

        public QuestionService(IConfiguration config)
        {
            _configuration = config;
            var connectionString = config.GetSection("MongoDB2:ConnectionString").Value;
            var databaseName = config.GetSection("MongoDB2:DatabaseName").Value;
            var collectionName = config.GetSection("MongoDB2:QuestionsCollectionName").Value;

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentNullException(nameof(connectionString), "MongoDB connection string is not configured.");
            }

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);
            _questions = database.GetCollection<Question1>(collectionName);
        }

        public async Task<List<Question1>> GetAsync() =>
            await _questions.Find(_ => true).ToListAsync();

        public async Task CreateAsync(Question1 question) =>
            await _questions.InsertOneAsync(question);

        public async Task<List<Question1>> GetQuestionsByCategoriesForRoomAsync(List<string> categories, int count, string roomPin)
        {
            if (_roomQuestions.ContainsKey(roomPin))
            {
                return _roomQuestions[roomPin];
            }

            try
            {
                if (categories == null || !categories.Any())
                {
                    throw new ArgumentException("At least one category must be provided.");
                }

                var filter = Builders<Question1>.Filter.In(q => q.Category, categories);
                var totalQuestions = await _questions.CountDocumentsAsync(filter);

                if (totalQuestions == 0)
                {
                    Console.WriteLine("No questions found for the selected categories.");
                    return new List<Question1>();
                }

                var sampleSize = (int)Math.Min(totalQuestions, count);

                var questions = await _questions.Aggregate()
                    .Match(filter)
                    .Sample(sampleSize)
                    .ToListAsync();

                if (questions.Count < count)
                {
                    Console.WriteLine($"Warning: Only {questions.Count} questions available for the selected categories out of {count} requested.");
                }

                questions = questions.Where(q => categories.Contains(q.Category)).ToList();

                // Store the questions for this room
                _roomQuestions[roomPin] = questions;

                return questions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving questions by categories: {ex.Message}");
                return new List<Question1>();
            }
        }
        public List<Question1> GetQuestionsForRoom(string roomPin)
        {
            if (_roomQuestions.TryGetValue(roomPin, out var questions))
            {
                return questions;
            }
            return new List<Question1>();
        }
        public async Task StoreQuestionsForRoom(string roomPin, List<Question1> questions)
        {
            _roomQuestions[roomPin] = questions;
        }

        public List<Question1> GetQuestionsByCategoriesForRoomAsync(string roomPin)
        {
            if (_roomQuestions.TryGetValue(roomPin, out var questions))
            {
                return questions;
            }
            return new List<Question1>();
        }
        public async Task<List<Question1>> GetQuestionsByCategoriesAsync(List<string> categories, int count)
        {
            try
            {
                if (categories == null || !categories.Any())
                {
                    throw new ArgumentException("At least one category must be provided.");
                }

                var filter = Builders<Question1>.Filter.In(q => q.Category, categories);
                var totalQuestions = await _questions.CountDocumentsAsync(filter);

                if (totalQuestions == 0)
                {
                    Console.WriteLine("No questions found for the selected categories.");
                    return new List<Question1>();
                }

                var sampleSize = (int)Math.Min(totalQuestions, count);

                var questions = await _questions.Aggregate()
                    .Match(filter)
                    .Sample(sampleSize)
                    .ToListAsync();

                if (questions.Count < count)
                {
                    Console.WriteLine($"Warning: Only {questions.Count} questions available for the selected categories out of {count} requested.");
                }

                return questions.Where(q => categories.Contains(q.Category)).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving questions by categories: {ex.Message}");
                return new List<Question1>();
            }
        }
        public async Task<List<Question1>> GetRandomQuestionsAsync(int count)
        {
            try
            {
                var questions = await _questions.Aggregate()
                    .Sample(count)
                    .ToListAsync();

                if (questions.Count < count)
                {
                    Console.WriteLine($"Warning: Only {questions.Count} questions retrieved out of {count} requested.");
                }

                return questions;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving questions: {ex.Message}");
                return new List<Question1>();
            }
        }
    }
}