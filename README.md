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
