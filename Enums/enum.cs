namespace Quiz_API.Enums
{
    public enum RepositoryResult
    {
        Success,
        QuizNotFound,
        QuestionNotFound,
        AnswerNotFound,
        AnswerIncorrect,
        AnswerCorrect,
        TooManyCorrectAnswers,
        Score
    }
    public enum UserRepositoryResult
    {
        Succes,
        UserNotCreated,
        UserNotFound,
        UserExsists,
        WrongPassword
    };
}
