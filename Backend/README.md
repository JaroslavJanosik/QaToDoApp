# Backend README

This folder contains the ASP.NET Core Web API for the QA ToDoApp.

## Tech stack

- ASP.NET Core Web API
- .NET 9
- Entity Framework Core
- EF Core InMemory provider
- AutoMapper
- Swagger / Swashbuckle
- xUnit for tests

## Folder structure

```text
Backend/
├── QaToDoApp/                  # Main API project
├── QaToDoAppIntegrationTests/  # Integration tests
├── QaToDoAppUnitTests/         # Unit tests
├── QaToDoApp.sln
└── README.md
```

## Main responsibilities

The backend provides REST endpoints for managing ToDo items:

- get all tasks
- get a task by id
- create a task
- update a task
- patch a task
- delete a task

## API project location

Main API project:

```text
Backend/QaToDoApp
```

## How to run locally

```bash
cd Backend/QaToDoApp

dotnet restore
dotnet run
```

Expected local URL:

```text
https://localhost:5111
```

Swagger UI:

```text
https://localhost:5111/swagger/index.html
```

## Database behavior

The application uses:

```text
UseInMemoryDatabase("ToDoList")
```

That means:

- no SQL Server setup is required for local development
- data is stored only in memory while the app is running
- restarting the API resets the data

## API endpoints

Base route:

```text
/api/ToDoItems
```

Endpoints:

- `GET /api/ToDoItems` - get all tasks
- `GET /api/ToDoItems/{id}` - get one task
- `POST /api/ToDoItems` - create a task
- `PUT /api/ToDoItems/{id}` - replace a task
- `PATCH /api/ToDoItems/{id}` - partially update a task
- `DELETE /api/ToDoItems/{id}` - delete a task

## Request examples

### Create task

```json
{
  "text": "Buy groceries"
}
```

### Update task

```json
{
  "id": 1,
  "text": "Buy groceries and milk",
  "completed": false
}
```

### Patch task

```json
[
  {
    "op": "replace",
    "path": "/text",
    "value": "Updated task text"
  }
]
```

## Important source files

- `QaToDoApp/Controllers/ToDoItemsController.cs` - API endpoints
- `QaToDoApp/Models/ToDoItem.cs` - domain model
- `QaToDoApp/Models/Dto/` - request and response DTOs
- `QaToDoApp/Data/ToDoDbContext.cs` - EF Core context
- `QaToDoApp/Repository/` - repository layer
- `QaToDoApp/Startup.cs` - DI, CORS, Swagger, controllers, DB setup
- `QaToDoApp/Program.cs` - app startup entry point

## CORS

The backend configures a permissive CORS policy named `AllowOrigins` that allows:

- any origin
- any method
- any header

This is convenient for local development, but for production it should be tightened.

## Running tests

Run all backend tests:

```bash
cd Backend
dotnet test
```

Or run from the solution file:

```bash
cd Backend
dotnet test QaToDoApp.sln
```

## Known limitations

- The app uses an in-memory database only.
- There is no authentication or authorization.
- CORS is fully open.
- Persistence does not survive an application restart.
