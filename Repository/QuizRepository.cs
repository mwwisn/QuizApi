using Quiz_API.Data;
using Quiz_API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Quiz_API.Migrations;
using System.Runtime.InteropServices;
using Microsoft.Identity.Client;
using static Quiz_API.Repository.IQuizRepository;
using Quiz_API.Enums;
using System.Collections.ObjectModel;
using Quiz_API.Nowy_folder;
using Quiz_API.Dto;

namespace Quiz_API.Repository
{
    public class QuizRepository : IQuizRepository
    {
        private readonly QuizContext _quizContext;
        public QuizRepository(QuizContext quizContext)
        {
            _quizContext = quizContext;
        }

        public async Task<Quiz> AddQuiz()
        {
            var quiz = new Quiz { };
            quiz.Questions = new List<Question>();
            _quizContext.Quizzes.Add(quiz);

            await _quizContext.SaveChangesAsync();
            return quiz;
        }

        public async Task<RepositoryResult> AddQuestion(int code, CreateQuestionDto createQuestionDto)
        {
            var quiz = await _quizContext.Quizzes.FirstOrDefaultAsync(x => x.Code == code);
            if (quiz == null)
            {
                return RepositoryResult.QuizNotFound;
            }
            var question = new Question
            {
                QuizCode = quiz.Code,
                QuestionName = createQuestionDto.QuestionName
            };
            await _quizContext.Questions.AddAsync(question);
            await _quizContext.SaveChangesAsync();
            return RepositoryResult.Success;
        }

        public async Task<IEnumerable<GetQuizesDto>> GetAllQuizes()
        {
            var response = new Collection<GetQuizesDto>();
            var quizes = await _quizContext.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.Answers)
            .ToListAsync();
            foreach(var quiz in quizes)
            {
                var QuizDto = new GetQuizesDto
                {
                    Code = quiz.Code
                };
                foreach (var question in quiz.Questions)
                {
                    var qst = new ListOfQuestion {
                        Id = question.Id,
                        QuestionName = question.QuestionName,
                    };
                    foreach (var answer in question.Answers)
                    {
                        var ans = new ListOfAns
                        {
                            Id = answer.Id,
                            AnswerName = answer.AnswerName,
                            IsCorrect = answer.IsCorrect
                            
                        };
                        qst.Answers.Add(ans);
                    }
                    QuizDto.Questions.Add(qst);
                }
                response.Add(QuizDto);
            }
            return response;
        }

        public async Task<Question> GetRandomQuestion(int code)
        {
            var quiz = await _quizContext.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(x => x.Code == code);
            if (quiz == null)
            {
                return null;
            }
            var randomQuestion = quiz.Questions.OrderBy(q => Guid.NewGuid()).FirstOrDefault();
            if(randomQuestion == null)
            {
                return null;
            }
            return randomQuestion;
        }
        public async Task<Collection<GetQuizDto>> GetQuiz(int code)
        {
            var quiz = new Collection<GetQuizDto>();
            var numberOfQuestion = 10;

            var quizEntity = await _quizContext.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(x => x.Code == code);

            if (quizEntity == null)
            {
                return quiz;
            }

            // wszystkie pytania, przetasowane
            var shuffledQuestions = quizEntity.Questions
                .OrderBy(q => Guid.NewGuid())
                .Take(numberOfQuestion) // bierz tylko potrzebną ilość
                .ToList();

            foreach (var randomQuestion in shuffledQuestions)
            {
                var correctAnswer = randomQuestion.Answers.FirstOrDefault(x => x.IsCorrect);

                if (correctAnswer != null)
                {
                    var updateQuota = new Ans
                    {
                        QuestionId = correctAnswer.QuestionId,
                        AnswerId = correctAnswer.Id
                    };
                    QuizSession.Ans.Add(updateQuota);
                }

                var response = new GetQuizDto
                {
                    Id = randomQuestion.Id,
                    QuestionName = randomQuestion.QuestionName
                };

                foreach (var answer in randomQuestion.Answers)
                {
                    response.Answers.Add(new ListOfAnswers
                    {
                        AnswerName = answer.AnswerName,
                        Id = answer.Id
                    });
                }

                quiz.Add(response);
            }

            return quiz;
        }
        
        public async Task<Collection<GetQuizToEditDto>> GetQuizToEdit(int code)
        {
            var response = new Collection<GetQuizToEditDto>();

            var quizEntity = await _quizContext.Quizzes.
                Include(q => q.Questions).
                ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(x => x.Code == code);
            if(quizEntity == null)
            {
                return response;
            }
            foreach(var questEntity in quizEntity.Questions)
            {
                var quest = new GetQuizToEditDto { };
                quest.Id = questEntity.Id;
                quest.QuestionName = questEntity.QuestionName;
                foreach(var AnswerEntity in questEntity.Answers)
                {
                    var answer = new AnswersToEdit { };
                    answer.Id = AnswerEntity.Id;
                    answer.AnswerName = AnswerEntity.AnswerName;
                    answer.IsCorrect = AnswerEntity.IsCorrect;
                    quest.AnsEdit.Add(answer);
                }
                response.Add(quest);
            }
            return response;
        }

        public async Task<ScoreDto> GetScore(Collection<UserAnswerDto> userAnswerDtos)
        {
            var response = new ScoreDto
            {
            };
            foreach (var userAnswer in userAnswerDtos)
            {
                var question = QuizSession.Ans.FirstOrDefault(x => x.QuestionId == userAnswer.QuestionId);
                if(question == null)
                {
                    response.Score = 0;
                    response.Status = "Blad podczas odczytywania pytan.";
                    return response;
                    
                }
                if(userAnswer.AnswerId == question.AnswerId)
                {
                    response.Score++;
                }
            }
            response.Status = "Udzielono odpowiedzi na pytania.";
            return response;
        }


        public async Task<RepositoryResult> CheckAnswer(int code, int questionId, int answerId)
        {
            var quiz = await
                _quizContext.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(x => x.Code == code);
            if (quiz == null)
            {
                return RepositoryResult.QuizNotFound;
            }
            var question = 
                quiz.Questions
                .FirstOrDefault(x => x.Id == questionId);
            if (question == null)
            {
                return RepositoryResult.QuestionNotFound;
            }
            var answer =
                question.Answers
                .FirstOrDefault(x => x.Id == answerId);
            if (answer == null)
            {
                return RepositoryResult.AnswerNotFound;
            }
            if (answer.IsCorrect == false)
            {
                return RepositoryResult.AnswerIncorrect;
            }
            return RepositoryResult.AnswerCorrect;
        }
        public async Task<RepositoryResult> AddAnswer(int code, int questId, CreateAnswerDto createAnswerDto)
        {
            var QuizExsist = await _quizContext.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(x => x.Code == code);
            if (QuizExsist == null)
            {
                return RepositoryResult.QuizNotFound;
            }
            var QuestionExist = QuizExsist.Questions.FirstOrDefault(x => x.Id == questId);
            if (QuestionExist == null)
            {
                return RepositoryResult.QuestionNotFound;
            }
            var answer = new Answer
            {
                QuestionId = questId,
                AnswerName = createAnswerDto.AnswerName,
                IsCorrect = createAnswerDto.IsCorrect
            };
            //one question only can have one correct answer
            var OneCorrect = answer.IsCorrect ? 1 : 0;
            var correctAnswersCount = QuestionExist.Answers
                .Count(a => a.IsCorrect == true);
            correctAnswersCount += OneCorrect;
            if (correctAnswersCount > 1)
            {
                return RepositoryResult.TooManyCorrectAnswers;
            }
            await _quizContext.Answers.AddAsync(answer);
            await _quizContext.SaveChangesAsync();
            return RepositoryResult.Success;
        }

        public async Task<RepositoryResult> UpdateQuestion(int code, int questId, Question question)
        {
            var quiz = await _quizContext.Quizzes.
            Include(q => q.Questions)
            .FirstOrDefaultAsync(x => x.Code == code);
            if (quiz == null)
            {
                return RepositoryResult.QuizNotFound;
            }
            var quest = quiz.Questions.FirstOrDefault(q => q.Id == questId);
            if(quest == null)
            {
                return RepositoryResult.QuestionNotFound;
            }
            quest.QuestionName = question.QuestionName;
            await _quizContext.SaveChangesAsync();
            return RepositoryResult.Success;
        }

        public async Task<RepositoryResult> UpdateAnswer(int code, int questionId, int answerId, Answer answer)
        {
            var QuizEntity =
                await _quizContext.Quizzes
                .Include(q => q.Questions)
                .ThenInclude(q => q.Answers)
                .FirstOrDefaultAsync(x => x.Code == code);
            if(QuizEntity == null)
            {
                return RepositoryResult.QuizNotFound;
            }
            var QuestionEntity = QuizEntity.Questions.FirstOrDefault(x => x.Id == questionId);
            if(QuestionEntity == null)
            {
                return RepositoryResult.QuestionNotFound;
            }
            var AnswerEntity = QuestionEntity.Answers.FirstOrDefault(x => x.Id == answerId);
            if(AnswerEntity == null)
            {
                return RepositoryResult.AnswerNotFound;
            }
            if (answer.IsCorrect)
            {
                foreach(var ans in QuestionEntity.Answers)
                {
                    if(ans.Id != answer.Id)
                        ans.IsCorrect = false;
                }
            }
            AnswerEntity.IsCorrect = answer.IsCorrect;
            AnswerEntity.AnswerName = answer.AnswerName;
            await _quizContext.SaveChangesAsync();
            return RepositoryResult.Success;
               
        }
        public async Task<RepositoryResult> DeleteQuiz(int code)
        {
            var quiz = await _quizContext.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.Answers).
            FirstOrDefaultAsync(q => q.Code == code);
            if (quiz == null)
            {
                return RepositoryResult.QuizNotFound;
            }
            foreach (var question in quiz.Questions)
            {
               _quizContext.RemoveRange(question.Answers);
            }
            _quizContext.Questions.RemoveRange(quiz.Questions);
            _quizContext.Remove(quiz);
            await _quizContext.SaveChangesAsync();
            return RepositoryResult.Success;
        }
        public async Task<RepositoryResult> DeleteQuestion(int code, int questionId)
        {
            var quiz = await
            _quizContext.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(x => x.Code == code);
            if (quiz == null)
            {
                return RepositoryResult.QuizNotFound;
            }
            var question =
                quiz.Questions
                .FirstOrDefault(x => x.Id == questionId);
            if (question == null)
            {
                return RepositoryResult.QuestionNotFound;
            }
            _quizContext.Answers.RemoveRange(question.Answers);
            _quizContext.Remove(question);
            await _quizContext.SaveChangesAsync();
            return RepositoryResult.Success;
        }

        public async Task<RepositoryResult> DeleteAnswer(int code, int questionId, int answerId)
        {
            var quiz = await 
                _quizContext.Quizzes
            .Include(q => q.Questions)
            .ThenInclude(q => q.Answers)
            .FirstOrDefaultAsync(x => x.Code == code);
            if (quiz == null)
            {
                return RepositoryResult.QuizNotFound;
            }
            var question = quiz.Questions
                .FirstOrDefault(x => x.Id == questionId);
            if (question == null)
            {
                return RepositoryResult.QuestionNotFound;
            }
            var answer = question.Answers
                .FirstOrDefault(x => x.Id == answerId);
            if (answer == null)
            {
                return RepositoryResult.AnswerNotFound;
            }
            _quizContext.Remove(answer);
            await _quizContext.SaveChangesAsync();
            return RepositoryResult.Success;
        }
    }
}