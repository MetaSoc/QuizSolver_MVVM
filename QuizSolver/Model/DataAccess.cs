using Dapper;
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
            var output = cnn.Query<Quiz>($"select * from {selectedQuiz}", new DynamicParameters());
            return output.ToList();
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
