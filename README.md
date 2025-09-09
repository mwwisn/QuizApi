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

Endpoint: POST /api/user/login

Body (JSON):
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
