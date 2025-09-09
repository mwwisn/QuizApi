# Dokumentacja API – Quiz_API
## Autoryzacja

- Wszystkie endpointy w QuizController wymagają Bearer Token w nagłówku Authorization.

- Token generowany przez endpoint POST /api/user/login

## UserController

### 1. Rejestracja użytkownika

- Endpoint: POST /api/user/register

- Body (JSON):
{
  "email": "user@example.com",
  "username": "username",
  "password": "password123"
}

### Odpowiedzi:
200 OK – User Created

400 Bad Request – User not created

500 Internal Server Error – Unexpected error.


## 2. Logowanie

- Endpoint: POST /api/user/login

- Body (JSON):
{
  "email": "user@example.com",
  "password": "password123"
}


### Odpowiedzi:

200 OK – zwraca token JWT:
{
  "token": "<JWT Token>"
}
400 Bad Request – User not found lub Wrong password

500 Internal Server Error – Błąd serwera

## QuizController

## 1. Pobranie wszystkich quizów

- Endpoint: GET /api/quiz

### Odpowiedzi:

200 OK – lista quizów

404 Not Found – Quiz not found

## 2. Pobranie losowego pytania z quizu

- Endpoint: GET /api/quiz/randomTask/{code}

- Parametry:

-- code – kod quizu

### Odpowiedzi:

200 OK – pojedyncze pytanie

404 Not Found – brak quizu lub pytania

## 3. Rozwiązywanie quizu

- Endpoint: GET /api/quiz/quiz/{code}

- Parametry:

code – kod quizu

## Odpowiedzi:

200 OK – pytania wraz z odpowiedziami

404 Not Found – brak pytań dla danego quizu

500 Internal Server Error – nieoczekiwany błąd

## 4. Pobranie quizu do edycji

- Endpoint: GET /api/quiz/edit-quiz/{code}

- Parametry:

code – kod quizu

### Odpowiedzi:

200 OK – quiz do edycji

404 Not Found – quiz nie znaleziony

500 Internal Server Error – nieoczekiwany błąd

## 5. Pobranie odpowiedzi sesji

- Endpoint: GET /api/quiz/session-answer

### Odpowiedzi:

200 OK – lista poprawnych odpowiedzi w sesji

## 6. Obliczenie wyniku quizu

- Endpoint: POST /api/quiz/submit-quiz

- Body (JSON):

[
  {
    "QuestionId": 1,
    "AnswerId": 3
  },
  ...
]


### Odpowiedzi:

200 OK – wynik i status:

{
  "Score": 7,
  "Status": "Udzielono odpowiedzi na pytania."
}


500 Internal Server Error – nieoczekiwany błąd

## 7. Sprawdzenie poprawności odpowiedzi

- Endpoint: GET /api/quiz/{code}/question/{questionId}/answer/{answerId}

### Odpowiedzi:

200 OK – Answer is correct

400 Bad Request – Answer is incorrect

404 Not Found – Quiz not found / Question not found / Answer not found

500 Internal Server Error – nieoczekiwany błąd

## 8. Tworzenie quizu

- Endpoint: POST /api/quiz/CreateQuiz

### Odpowiedzi:

201 Created – quiz utworzony, zwraca code

## 9. Dodawanie pytania do quizu

- Endpoint: POST /api/quiz/{code}/questions

- Body: CreateQuestionDto

### Odpowiedzi:

200 OK – Question added

404 Not Found – Quiz not found

500 Internal Server Error – nieoczekiwany błąd

## 10. Dodawanie odpowiedzi do pytania

- Endpoint: POST /api/quiz/{code}/question/{questId}

- Body: CreateAnswerDto

### Odpowiedzi:

204 No Content – poprawnie dodano

400 Bad Request – Only one correct answer is allowed

404 Not Found – Quiz/Question not found

500 Internal Server Error – nieoczekiwany błąd

## 11. Aktualizacja pytania

- Endpoint: PUT /api/quiz/{code}/question/{questionId}

- Body: Question

### Odpowiedzi:

200 OK – Question changed.

404 Not Found – Quiz/Question not found

500 Internal Server Error – nieoczekiwany błąd

## 12. Aktualizacja odpowiedzi

- Endpoint: PUT /api/quiz/{code}/question/{questionId}/{answerId}

- Body: Answer

### Odpowiedzi:

200 OK – Answer Updated

400 Bad Request – One question can have only one correct answer

404 Not Found – Quiz/Question/Answer not found

500 Internal Server Error – nieoczekiwany błąd

## 13. Usuwanie quizu

- Endpoint: DELETE /api/quiz/{code}

### Odpowiedzi:

200 OK – Quiz Deleted.

404 Not Found – Quiz not found

500 Internal Server Error – nieoczekiwany błąd

##14. Usuwanie pytania
 
- Endpoint: DELETE /api/quiz/{code}/question/{questionId}

### Odpowiedzi:

200 OK – Question deleted.

404 Not Found – Quiz/Question not found

500 Internal Server Error – nieoczekiwany błąd

## 15. Usuwanie odpowiedzi

- Endpoint: DELETE /api/quiz/{code}/question/{questionId}/answer/{answerId}

### Odpowiedzi:

200 OK – Answer deleted.

404 Not Found – Quiz/Question/Answer not found

500 Internal Server Error – nieoczekiwany błąd
