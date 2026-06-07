# DotNetLearning — Task API

ASP.NET Core Web API portfolio project: task management with EF Core, JWT authentication, repository pattern, and xUnit tests.

Built while transitioning from VB.NET / ASP.NET Web Forms and Xamarin/MAUI to modern .NET.

## Tech stack

- **.NET 10** / ASP.NET Core Web API
- **Entity Framework Core** + SQLite
- **JWT Bearer** authentication
- **xUnit**, **Moq**, EF Core InMemory (tests)
- **Visual Studio** + `.http` file for API testing

## Features

- User registration and login (password hashing)
- JWT-protected task endpoints
- Full CRUD on tasks
- Optional filter: `GET /api/tasks?isComplete=true`
- 14 unit tests (repository, controller, auth)

## Getting started

### Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download) or later
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) (recommended)

### Run the API

```powershell
git clone https://github.com/AndrewRodman/DotNetLearning.git
cd DotNetLearning
dotnet run --project TaskApi
```

Or open `DotNetLearning.slnx` in Visual Studio, set **TaskApi** as startup project, press **F5**.

### Run tests

```powershell
dotnet test
```

### Try the API (Visual Studio)

1. Run **TaskApi** (F5)
2. Open `TaskApi/TaskApi.http`
3. Run **Register** (step 1) — once per fresh database
4. Run **Login** (step 2)
5. Run task requests (step 3) — token is picked up from the login response

If login fails after schema changes, reset the local database:

```powershell
.\reset-db.ps1
```

Then F5 → Register → Login again.

## API endpoints

### Auth (public)

| Method | URL | Description |
|--------|-----|-------------|
| POST | `/api/auth/register` | Create account, returns JWT |
| POST | `/api/auth/login` | Login, returns JWT |

### Tasks (requires `Authorization: Bearer <token>`)

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/api/tasks` | List all tasks |
| GET | `/api/tasks?isComplete=true` | Filter by completion |
| GET | `/api/tasks/{id}` | Get one task |
| POST | `/api/tasks` | Create a task |
| PUT | `/api/tasks/{id}` | Update a task |
| DELETE | `/api/tasks/{id}` | Delete a task |

## Project structure

```
TaskApi/
  Controllers/     AuthController, TasksController
  Services/        AuthService (JWT + passwords)
  Repositories/    TaskRepository (data access)
  Data/            AppDbContext (EF Core)
  Models/          TaskItem, User, request DTOs
TaskApi.Tests/     xUnit tests
```

## Architecture

```
HTTP Request
  → Middleware (Authentication / Authorization)
  → Controller
  → Repository
  → EF Core DbContext
  → SQLite (tasks.db, created locally)
```

## Configuration

JWT settings are in `TaskApi/appsettings.json` under `"Jwt"`. The included key is for **local development only**. In production, use environment variables or user secrets — never commit real secrets.

## License

MIT — use freely for learning and portfolio reference.