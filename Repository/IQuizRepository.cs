using Quiz_API.Dto;
using Quiz_API.Enums;
using Quiz_API.Models;
using Quiz_API.Nowy_folder;
using System.Collections.ObjectModel;

namespace Quiz_API.Repository
{
    public interface IQuizRepository
    {
        public Task<IEnumerable<GetQuizesDto>> GetAllQuizes();
        public Task<Question> GetRandomQuestion(int Code);

        public Task<RepositoryResult> CheckAnswer(int code, int questionId, int answerId);
        public Task<Quiz> AddQuiz();
        public Task<RepositoryResult> AddQuestion(int code, CreateQuestionDto createQuestionDto);

        public Task<RepositoryResult> AddAnswer(int code, int questId, CreateAnswerDto createAnswerDto);

        public Task<RepositoryResult> UpdateQuestion(int code, int questId, Question question);

        public Task<RepositoryResult> UpdateAnswer(int code, int questionId, int answerId, Answer answer);

        public Task<RepositoryResult> DeleteQuiz(int code);

        public Task<RepositoryResult> DeleteQuestion(int code, int questionId);

        public Task<RepositoryResult> DeleteAnswer(int code, int questionId, int answerId);

        public Task<Collection<GetQuizDto>> GetQuiz(int code);

        public Task<Collection<GetQuizToEditDto>> GetQuizToEdit(int code);
        public Task<ScoreDto> GetScore(Collection<UserAnswerDto> userAnswerDtos);

    }
}
