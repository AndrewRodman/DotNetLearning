# DotNetLearning

A hands-on learning project for modern .NET, built for developers coming from VB.NET / ASP.NET Web Forms and Xamarin/MAUI.

## What's in here

| Project | Purpose |
|---------|---------|
| `TaskApi` | ASP.NET Core Web API with EF Core (SQLite) |
| `TaskApi.Tests` | xUnit test project (starter) |

## Concepts this project teaches

- **Program.cs** — replaces `Global.asax` + `web.config` startup
- **Dependency injection** — services registered in `Program.cs`, injected into controllers
- **Controllers** — similar to ASP.NET MVC/Web API you may have seen
- **EF Core** — modern data access (like EF6, but different APIs)
- **Repository pattern** — keeps data access out of controllers
- **appsettings.json** — replaces much of `web.config`

## Prerequisites

- .NET SDK (you have 10.x installed)
- Visual Studio 2022 (optional but recommended)

## Quick start

```powershell
cd C:\Users\Latitude5440\Projects\DotNetLearning
dotnet build
dotnet run --project TaskApi
```

Then open Swagger/OpenAPI in the browser (URL shown in the terminal), or use the `TaskApi.http` file in Visual Studio.

## API endpoints

| Method | URL | Description |
|--------|-----|-------------|
| GET | `/api/tasks` | List all tasks |
| GET | `/api/tasks/{id}` | Get one task |
| POST | `/api/tasks` | Create a task |
| PUT | `/api/tasks/{id}` | Update a task |
| PUT | `/api/tasks/{id}` | Mark complete, change title, etc. |
| DELETE | `/api/tasks/{id}` | Delete a task |

## Suggested learning path

### Week 1 — Understand the structure
1. Read `TaskApi/Program.cs` — ask Grok how this maps to Web Forms startup
2. Trace a request: `TasksController` → `ITaskRepository` → `AppDbContext`
3. Hit the API with `TaskApi.http` or Swagger

### Week 2 — Extend it
1. Add validation and better error responses
2. Add unit tests in `TaskApi.Tests`
3. Add filtering (`?isComplete=true`)

### Week 3 — Go further
1. Add JWT authentication
2. Add a .NET MAUI client that calls this API
3. Push to GitHub for your portfolio

## Open in Visual Studio

```
File → Open → Project/Solution → C:\Users\Latitude5440\Projects\DotNetLearning\DotNetLearning.slnx
```

## Work with Grok Build

```powershell
cd C:\Users\Latitude5440\Projects\DotNetLearning
grok
```

Example prompts:
- `@TaskApi/Program.cs explain this like I'm used to Global.asax`
- `Add unit tests for TasksController`
- `Add JWT authentication to this API`