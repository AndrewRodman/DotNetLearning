# DotNetLearning — Full-Stack Task Manager

[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Tests](https://img.shields.io/badge/tests-18%20passing-success)](https://github.com/AndrewRodman/DotNetLearning#features)
[![Live API](https://img.shields.io/badge/API-live-0078D4?logo=microsoft-azure&logoColor=white)](https://taskapi-andrew.azurewebsites.net)

Portfolio project: ASP.NET Core Web API, .NET MAUI client, SQL Server, JWT auth, and Azure deployment.

Built while transitioning from VB.NET / ASP.NET Web Forms and Xamarin/MAUI to modern .NET.

## Live demo

![TaskApp on Windows](docs/taskapp-screenshot.png)

- **API:** https://taskapi-andrew.azurewebsites.net
- **Try it:** `GET /health` returns `200 OK` (no auth). `GET /api/tasks` returns `401 Unauthorized` without a token (expected)
- **Database:** Azure SQL (free tier)

## Tech stack

| Layer | Technology |
|-------|------------|
| API | ASP.NET Core Web API (.NET 10) |
| Client | .NET MAUI (`TaskApp`) |
| Data | EF Core, SQL Server (LocalDB dev / Azure SQL prod) |
| Auth | JWT Bearer |
| Cloud | Azure App Service (Free F1) + Azure SQL |
| Tests | xUnit, Moq, `WebApplicationFactory` integration tests |

## Features

- User registration and login (password hashing)
- Health check endpoint (`GET /health`, includes database check)
- JWT-protected task endpoints
- Full CRUD on tasks
- Optional filter: `GET /api/tasks?isComplete=true`
- MAUI app — login, list tasks, add tasks, mark complete, delete tasks
- EF Core migrations (SQLite → SQL Server)
- **18 tests** — 14 unit + 4 integration (health, auth pipeline, create, delete)

## Getting started

### Prerequisites

- [.NET SDK 10](https://dotnet.microsoft.com/download) or later
- [Visual Studio 2022+](https://visualstudio.microsoft.com/) (recommended)
- SQL Server LocalDB (ships with Visual Studio) for local API development

### Clone and test

```powershell
git clone https://github.com/AndrewRodman/DotNetLearning.git
cd DotNetLearning
dotnet test
```

### Run locally (API + MAUI)

**1. API** — set **TaskApi** as startup, press **Ctrl+F5**

Uses LocalDB via `appsettings.Development.json`. Test with `TaskApi/TaskApi.http`:

1. **Register** → **Login** → task requests

Reset local database after schema changes:

```powershell
.\reset-db.ps1
```

**2. MAUI** — `TaskApp/Configuration/ApiSettings.cs` points at the live Azure API by default.

To run fully local instead, change `BaseUrl` to `http://localhost:5046/` and start TaskApi first, then F5 **TaskApp**.

### Deployed API (no local API needed)

The MAUI app can talk to Azure directly. F5 **TaskApp**, register a user, create tasks.

## API endpoints

### Health (public)

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/health` | API and database health check |

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
TaskApi/              ASP.NET Core Web API
TaskApp/              .NET MAUI client
TaskApi.Tests/        Unit + integration tests
```

## Architecture

```
MAUI / HTTP client
  → ASP.NET Core API (Azure App Service)
    → JWT middleware
    → Controller
    → Repository
    → EF Core DbContext
    → SQL Server (Azure SQL or LocalDB)
```

## Configuration

- **Local:** connection string in `appsettings.Development.json`, JWT in `appsettings.json`
- **Azure:** connection string and JWT in App Service environment variables (not in source control)

## License

MIT — use freely for learning and portfolio reference.