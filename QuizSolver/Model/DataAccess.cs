using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Dapper;
using QuizSolver.ViewModel;

namespace QuizSolver.Model
{
    public class DataAccess
    {
        public static List<string> LoadQuizzesList()
        {
            using SQLiteConnection conn = new(ConfigurationManager.
                ConnectionStrings["Default"].ConnectionString);

            List<string> quizzesList = new();

            conn.Open();

            DataTable dt = conn.GetSchema("Tables");
            foreach (DataRow row in dt.Rows)
            {
                if (row[2].ToString() != "sqlite_sequence")
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
