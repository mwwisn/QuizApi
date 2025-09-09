using Quiz_API.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Quiz_API.Dto
{
    public class GetQuizDto
    {
        public int Id { get; set; }
        public string QuestionName { get; set; }
        public List<ListOfAnswers> Answers { get; set; } = new List<ListOfAnswers>();
    }
    public class ListOfAnswers
    {
        public int Id { get; set; }
        public string AnswerName { get; set; }
    }
}
