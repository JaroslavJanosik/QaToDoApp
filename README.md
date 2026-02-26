# QA ToDoApp

A simple full-stack ToDo application with:

- **Frontend:** React + TypeScript
- **Backend:** ASP.NET Core Web API (.NET 9)
- **Storage:** EF Core InMemory database

This repository contains the full application, plus separate documentation for the frontend and backend.

## Repository structure

```text
QaToDoApp/
├── Backend/
│   ├── README.md
│   ├── QaToDoApp/                  # ASP.NET Core API
│   ├── QaToDoAppIntegrationTests/  # Integration tests
│   └── QaToDoAppUnitTests/         # Unit tests
├── Frontend/
│   ├── README.md
│   └── src/                        # React application source
├── docker-compose.yml
├── nginx.conf
└── README.md
```

## What the application does

The app lets users:

- create tasks
- view all tasks
- mark tasks as completed or open
- edit task text
- delete tasks

## Requirements

- **.NET SDK:** 9.0
- **Node.js:** compatible with the installed frontend dependencies
- **npm**

## Quick start

### 1. Run the backend

```bash
cd Backend/QaToDoApp

dotnet restore
dotnet run
```

By default, the API is expected at:

```text
https://localhost:5111
```

Swagger UI should be available at:

```text
https://localhost:5111/swagger/index.html
```

### 2. Run the frontend

Open a second terminal:

```bash
cd Frontend

npm install
npm start
```

The frontend should open at:

```text
http://localhost:3000
```

## Frontend to backend connection

The frontend reads the API base URL from:

```text
REACT_APP_API_URL
```

Example:

```bash
REACT_APP_API_URL=https://localhost:5111
```

If the variable is not set, the frontend falls back to:

```text
https://localhost:5111
```

## Testing

### Frontend

```bash
cd Frontend
npm test
```

### Backend

Run all backend tests from the `Backend` folder:

```bash
cd Backend
dotnet test
```

## Additional documentation

- [Frontend README](./Frontend/README.md)
- [Backend README](./Backend/README.md)

## Notes

- The backend currently uses an **InMemory** database, so data is reset when the API restarts.
- The repository also contains Docker and deployment-related files, but the simplest way to run the app locally is with `dotnet run` and `npm start`.
