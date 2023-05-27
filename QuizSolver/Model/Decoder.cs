using System;
using System.Text;

namespace QuizSolver.Model
{
    internal class Decoder
    {
        // Console.OutputEncoding = System.Text.Encoding.UTF8;
        public string Encode(string plainText)
        {
            var codedText = Convert.ToBase64String(Encoding.UTF8.GetBytes(plainText));
            return codedText;
        }

        public string Decode(string codedText)
        {
            var plainText = Encoding.UTF8.GetString(Convert.FromBase64String(codedText));
            return plainText;
        }
    }
}
