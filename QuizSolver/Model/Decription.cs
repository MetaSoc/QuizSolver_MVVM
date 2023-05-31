using System;
using System.Text;

namespace QuizSolver.Model
{
    internal class Decription
    {
        public static string Decript(string codedText)
        {
            var plainText = Encoding.UTF8.GetString(Convert.FromBase64String(codedText));
            return plainText;
        }

        // Encription
        /*public static string Encript(string plainText)
        {
            var codedText = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            return codedText;
        }*/
    }
}
