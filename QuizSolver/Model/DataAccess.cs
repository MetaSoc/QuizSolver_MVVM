using Dapper;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;

namespace QuizSolver.Model
{
    public class DataAccess
    {
        public static ObservableCollection<string> LoadQuizzesList()
        {
            using SQLiteConnection conn = new(LoadConnectionString());

            ObservableCollection<string> quizzesList = new();

            conn.Open();

            DataTable dt = conn.GetSchema("Tables");
            foreach (DataRow row in dt.Rows)
            {
                if (!row[2].ToString().Contains("sqlite"))
                    quizzesList.Add(row[2].ToString());
            }

            conn.Close();

            return quizzesList;
        }

        public static List<Quiz> LoadQuestions(string selectedQuiz)
        {
            using IDbConnection cnn = new SQLiteConnection(LoadConnectionString());
            var output = cnn.Query<Quiz>($"select * from {selectedQuiz}", new DynamicParameters()).ToList();

            // Shuffling answers TODO May simplify
            var random = new Random();
            foreach (var question in output)
            {
                var answers = new string[4];
                
                // Assigning answers to new array
                answers[0] = question.Answer1;
                answers[1] = question.Answer2;
                answers[2] = question.Answer3;
                answers[3] = question.Answer4;
                
                // Shuffling new array
                var correct = answers[question.CorrectAnswer - 1];
                var shuffledAnswers = answers.OrderBy(_ => random.Next()).ToArray();

                // Assigning shuffled answers
                question.Answer1 = shuffledAnswers[0];
                question.Answer2 = shuffledAnswers[1];
                question.Answer3 = shuffledAnswers[2];
                question.Answer4 = shuffledAnswers[3];

                // Assigning correct answer
                if (correct == question.Answer1)
                    question.CorrectAnswer = 1;
                else if (correct == question.Answer2)
                    question.CorrectAnswer = 2;
                else if (correct == question.Answer3)
                    question.CorrectAnswer = 3;
                else if (correct == question.Answer4)
                    question.CorrectAnswer = 4;
            }

            return output;
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
