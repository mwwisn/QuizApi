namespace Quiz_API.Dto
{
    public class GetQuizesDto
    {
        public int Code { get; set; }
        public List<ListOfQuestion> Questions { get; set; } = new List<ListOfQuestion>();

    }
    public class ListOfQuestion
    {
        public int Id { get; set; }
        public string QuestionName { get; set; }
        public List<ListOfAns> Answers { get; set; } = new List<ListOfAns>();
    }
    public class ListOfAns
    {
        public int Id { get; set; }
        public string AnswerName { get; set; }
        public bool IsCorrect { get; set; }
    }
}
