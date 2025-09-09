using System.ComponentModel.DataAnnotations;

namespace Quiz_API.Dto
{
    public class CreateAnswerDto
    {
        [Required]
        public string AnswerName { get; set; }
        [Required]
        public bool IsCorrect { get; set; }
    }
}
