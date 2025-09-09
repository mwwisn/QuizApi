using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Quiz_API.Models
{
    public class Quiz
    {
        [Key]
        public int Code { get; set; }
        public List<Question> Questions { get; set; }
    }
    public class Question
    {
        [Key]
        public int Id { get; set; }
        public string QuestionName { get; set; }
        [ForeignKey("Quiz")]
        public int QuizCode { get; set; }
        [JsonIgnore]
        public Quiz? Quiz { get; set; }
        public List<Answer> Answers { get; set; }
    }
    public class Answer
    {
        [Key]
        public int Id { get; set; }
        public string AnswerName { get; set; }
        public bool IsCorrect { get; set; }
        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        [JsonIgnore]
        public Question? Question { get; set; }
    }
}
