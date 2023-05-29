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

namespace QuizSolver.Model
{
    public class DataAccess
    {
        //static SQLiteConnection _conn = new(@"Data Source=.\DataBaseQuizzes.db; Version=3");
        ///*SQLiteDataReader reader;
        //SQLiteCommand command;*/

        //public static ObservableCollection<string> LoadQuizzesList()
        //{
        //    ObservableCollection<string> quizzes = new();
        //    _conn.Open();

        //    DataTable dt = _conn.GetSchema("Tables");
        //    foreach (DataRow row in dt.Rows)
        //    {
        //        quizzes.Add(row[2].ToString());
        //    }

        //    _conn.Close();
        //    return quizzes;
        //}

        public static List<Quiz> LoadQuestions()
        {
            using IDbConnection cnn = new SQLiteConnection(LoadConnectionString());
            var output = cnn.Query<Quiz>("select * from Quiz1", new DynamicParameters());
            return output.ToList();
        }

        private static string LoadConnectionString(string id = "Default")
        {
            return ConfigurationManager.ConnectionStrings[id].ConnectionString;
        }
    }
}
