using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Quiz_API.Data;
using Quiz_API.Dto;
using Quiz_API.Enums;
using Quiz_API.Models;
using Quiz_API.Nowy_folder;
using Quiz_API.Repository;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Security.Permissions;
using System.Xml.Linq;
using static Quiz_API.Repository.IQuizRepository;
namespace Quiz_API.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class QuizController : ControllerBase
    {

        private readonly IQuizRepository _quizRepository;

        public QuizController(IQuizRepository quizRepository)
        {
            _quizRepository = quizRepository;
        }
        //endpoint do zwracania wszystkich danych o quizach
        [HttpGet]
        public async Task<ActionResult<IEnumerable<GetQuizesDto>>> GetQuiz()
        {
            var quizes = await _quizRepository.GetAllQuizes();
            if(quizes.Any() == false)
            {
                return NotFound("Quiz not found");
            }
            return Ok(quizes);
        }

        //endpoint do zwracania losowego zadania z quizu
        [HttpGet("randomTask/{code}")]
        public async Task<ActionResult<Question>> GetTask(int code)
        {
            var randomQuestion = await _quizRepository.GetRandomQuestion(code);
            if (randomQuestion == null)
            {
                return NotFound();
            }
            
            return Ok(randomQuestion);
            
        }
        //endpoint do robienia quizu
        [HttpGet("quiz/{code}")]
        public async Task<ActionResult<Collection<GetQuizDto>>> GetQuizAns(int code)
        {
            try
            {
                var request = await _quizRepository.GetQuiz(code);
                if (request.Any() == false)
                {
                    return NotFound("Not found question for this quiz.");
                }
                return Ok(request);
            } catch (Exception ex)
            {
                return StatusCode(500, "Unexpected error.");
            }
        }
        [HttpGet("edit-quiz/{code}")]
        public async Task<ActionResult<Collection<GetQuizToEditDto>>> GetQuizToEdit(int code)
        {
            try
            {
                var request = await _quizRepository.GetQuizToEdit(code);
                if(request.Any() == false)
                {
                    return NotFound("quiz not found");
                }
                return Ok(request);
            } catch(Exception ex)
            {
                return StatusCode(500, "Unexpected error");
            }
        }
        [HttpGet("session-answer")]
        public async Task<ActionResult> GetAnswers()
        {
            return Ok(QuizSession.Ans);
        }
        [HttpPost("submit-quiz")]
        public async Task<ActionResult<ScoreDto>> GetScore([FromBody] Collection<UserAnswerDto> userAnswerDtos)
        {
            try
            {
                var request = await _quizRepository.GetScore(userAnswerDtos);
                return request;
            } catch(Exception ex)
            {
                return StatusCode(500, "Unexpected error.");
            }
        }
        //endpoint do sprawdzenia czy odpowiedz do danego zadania jest prawidlowa
        [HttpGet("{code}/question/{questionId}/answer/{answerId}")]
        public async Task<ActionResult> CheckAnswer(int code, int questionId, int answerId)
        {
            var result = await _quizRepository.CheckAnswer(code, questionId, answerId);
            return result switch
            {
                RepositoryResult.AnswerCorrect => Ok("Answer is correct"),
                RepositoryResult.QuizNotFound => NotFound("Quiz not found."),
                RepositoryResult.QuestionNotFound => NotFound("Question not found."),
                RepositoryResult.AnswerNotFound => NotFound("Answer not found."),
                RepositoryResult.AnswerIncorrect => BadRequest("Answer is incorrect"),
                _ => StatusCode(500, "Unexpected error.")
            };
            
        }
        //create quiz
        [HttpPost("CreateQuiz")]
        public async Task<ActionResult> CreateQuiz()
        {
            var quiz = await _quizRepository.AddQuiz();
            return CreatedAtAction(nameof(GetQuiz), new { code = quiz.Code }, quiz);
        }
        //create question
        [HttpPost("{code}/questions")]
        public async Task<ActionResult> CreateQuestion(int code, [FromBody] CreateQuestionDto createQuestionDto)
        {
            var result = await _quizRepository.AddQuestion(code, createQuestionDto);
            return result switch {
                RepositoryResult.QuizNotFound => NotFound("Quiz not found"),
                RepositoryResult.Success => Ok("Question added"),
                _ => StatusCode(500, "Unexcpected error.")
            };
        }
        //add answer to your question
        [HttpPost("{code}/question/{questId}")]
        public async Task<ActionResult> AddAnswer(int code, int questId, [FromBody] CreateAnswerDto createAnswerDto)
        {
            var result = await _quizRepository.AddAnswer(code, questId, createAnswerDto);
            return result switch {
                RepositoryResult.Success => NoContent(),
                RepositoryResult.QuizNotFound => NotFound("Quiz not found."),
                RepositoryResult.QuestionNotFound => NotFound("Question not found."),
                RepositoryResult.TooManyCorrectAnswers => BadRequest("Only one correct answer is allowed."),
                _ => StatusCode(500, "Unexpected error.")
            };

        }
        //update your question
        [HttpPut("{code}/question/{questionId}")]
        public async Task<ActionResult> UpdateQuestion(int code, int questionId, [FromBody] Question question)
        {
            var result = await _quizRepository.UpdateQuestion(code, questionId, question);
            return result switch {
                RepositoryResult.QuizNotFound => NotFound("Quiz not found."),
                RepositoryResult.QuestionNotFound => NotFound("Question not found."),
                RepositoryResult.Success => Ok("Question changed."),
                _ => StatusCode(500,"Unexpected error.")
            };

        }
        //update your answear
        [HttpPut("{code}/question/{questionId}/{answerId}")]
        public async Task<ActionResult> UpdateAnswer(int code, int questionId, int answerId, [FromBody] Answer answer)
        {
            var result = await _quizRepository.UpdateAnswer(code, questionId, answerId, answer);
            return result switch {
                RepositoryResult.Success => Ok("Answer Updated"),
                RepositoryResult.TooManyCorrectAnswers => BadRequest("One question can have only one correct answer"),
                RepositoryResult.QuizNotFound => NotFound("Quiz not found."),
                RepositoryResult.QuestionNotFound => NotFound("Question not found."),
                RepositoryResult.AnswerNotFound => NotFound("Answer not found."),
                _ => StatusCode(500, "Unexpected error.")
            };
        }
        //Delete your quiz
        [HttpDelete("{code}")]
        public async Task<IActionResult> DeleteQuiz(int code)
        {
            var result = await _quizRepository.DeleteQuiz(code);
            return result switch {
                RepositoryResult.Success => Ok("Quiz Deleted."),
                RepositoryResult.QuizNotFound => NotFound("Quiz not found."),
                _ => StatusCode(500, "Unexpected error")
            };
        }
        //Delete your question
        [HttpDelete("{code}/question/{questionId}")]
        public async Task<ActionResult> DeleteQuestion(int code, int questionId)
        {
            var result = await _quizRepository.DeleteQuestion(code, questionId);
            return result switch
            {
                RepositoryResult.Success => Ok("Question deleted."),
                RepositoryResult.QuizNotFound => NotFound("Quiz not found."),
                RepositoryResult.QuestionNotFound => NotFound("Question not found."),
                _ => StatusCode(500, "Unexpected error.")
            };
               
        }
        //Delete your answer
        [HttpDelete("{code}/question/{questionId}/answer/{answerId}")]
        public async Task<ActionResult> DeleteAnswer(int code, int questionId, int answerId)
        {
            var result = await _quizRepository.DeleteAnswer(code, questionId, answerId);
            return result switch
            {
                RepositoryResult.Success => Ok("Answer deleted."),
                RepositoryResult.QuizNotFound => NotFound("Quiz not found."),
                RepositoryResult.QuestionNotFound => NotFound("Question not found."),
                RepositoryResult.AnswerNotFound => NotFound("Answer not found."),
                _ => StatusCode(500, "Unexpected error.")
            };
        }

    }
}
