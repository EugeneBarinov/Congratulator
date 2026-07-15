# Поздравлятор

Учебный проект — уровень 5. Приложение для ведения списка дней рождений друзей, коллег и знакомых.

## Стек

- **Backend:** ASP.NET Core 8 Web API, EF Core 8 + SQLite
- **Frontend:** React 18, Vite, React Router, Axios
- **Рассылка:** BackgroundService + SMTP (System.Net.Mail)
- **Тесты:** xUnit

## Функциональность

- Главная страница — сегодняшние и ближайшие дни рождения
- Список всех записей с поиском
- Добавление, редактирование, удаление записей
- Загрузка и отображение фотографий именинников
- Автоматическая email-рассылка дайджеста по расписанию

## Структура проекта

```
src/Congratulator.Api/     — ASP.NET Core Web API
  Models/                  — сущность Person
  Common/                  — логика расчёта дат (BirthdayCalculator)
  Abstractions/            — интерфейсы (DIP)
  Data/                    — AppDbContext (EF Core)
  Repositories/            — EfPersonRepository
  Services/                — PersonService, BirthdayNotificationService
  Storage/                 — LocalPhotoStorage
  Notifications/           — SmtpMessageSender, ConsoleMessageSender
  BackgroundServices/      — BirthdayReminderHostedService
  Controllers/             — PeopleController, NotificationsController

tests/Congratulator.Tests/ — xUnit тесты (BirthdayCalculator)

client/                    — React SPA (Vite)
  src/pages/               — HomePage, AllPeoplePage, PersonFormPage
  src/components/          — NavBar, PersonCard, EmptyState, ConfirmDialog
  src/api/client.js        — axios-клиент
```

## Запуск

Требования: [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) и [Node.js 18+](https://nodejs.org)

### Быстрый запуск (Windows)

Запустить `start.ps1` — скрипт сам поднимет backend и frontend и откроет браузер.

### Вручную

**Backend** (в первом терминале):
```bash
cd src/Congratulator.Api
dotnet run
```
API запустится на `http://localhost:5080`, Swagger — на `http://localhost:5080/swagger`.

**Frontend** (во втором терминале):
```bash
cd client
npm install
npm run dev
```
Приложение откроется на `http://127.0.0.1:5173`.

### Тесты
```bash
cd tests/Congratulator.Tests
dotnet test
```

## Настройка email-рассылки

По умолчанию рассылка отключена. Чтобы включить — необходимо отредактировать секцию `Notifications` в `src/Congratulator.Api/appsettings.json`:

```json
"Notifications": {
  "Enabled": true,
  "DailyRunTime": "09:00",
  "UpcomingDaysThreshold": 3,
  "Recipients": [ "your@email.com" ],
  "Smtp": {
    "Host": "smtp.gmail.com",
    "Port": 587,
    "User": "your@gmail.com",
    "Password": "пароль приложения",
    "FromAddress": "your@gmail.com",
    "EnableSsl": true
  }
}
```

Если `Smtp:Host` пустой — письма не отправляются, а пишутся в консоль. Это удобно для демонстрации без настройки почты.

Проверить рассылку без ожидания расписания: `POST http://localhost:5080/api/notifications/run-now`
