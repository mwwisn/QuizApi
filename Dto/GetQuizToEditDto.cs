namespace Quiz_API.Dto
{
    public class GetQuizToEditDto
    {
        public int Id { get; set; }
        public string QuestionName { get; set; }
        public List<AnswersToEdit> AnsEdit { get; set; } = new List<AnswersToEdit>();
    }
    public class AnswersToEdit
    {
        public int Id { get; set; }
        public string AnswerName { get; set; }
        public bool IsCorrect { get; set; }
    }
}
