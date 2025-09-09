using Quiz_API.Models;
using System.Collections.ObjectModel;

namespace Quiz_API.Nowy_folder
{
    public static class QuizSession
    {
        public static Collection<Ans> Ans { get; set; } = new Collection<Ans>();
    }
    public class Ans
    {
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public int UserAnswer { get; set; }
    }
}
