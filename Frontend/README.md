# Frontend README

This folder contains the React frontend for the QA ToDoApp.

## Tech stack

- React 18
- TypeScript
- Axios
- Font Awesome
- Create React App
- Cypress and React Testing Library dependencies

## What the frontend does

The frontend provides the user interface to:

- display all tasks
- create a new task
- edit an existing task
- mark a task as completed or open
- delete a task
- show loading, error, and empty states

## How to run locally

```bash
cd Frontend

npm install
npm start
```

Local development URL:

```text
http://localhost:3000
```

## Available scripts

### Start development server

```bash
npm start
```

### Run tests

```bash
npm test
```

### Create production build

```bash
npm run build
```

## Environment configuration

The frontend uses the following environment variable:

```text
REACT_APP_API_URL
```

Example usage:

```bash
REACT_APP_API_URL=https://localhost:5111 npm start
```

If the variable is not provided, the app uses this default API URL:

```text
https://localhost:5111
```

## Important files

- `src/config.tsx` - API URL configuration
- `src/components/ToDoWrapper.tsx` - main state and API orchestration
- `src/components/ToDoForm.tsx` - create task form
- `src/components/EditToDoForm.tsx` - edit task form
- `src/components/ToDoItem.tsx` - individual task UI
- `src/App.tsx` - app entry UI
- `src/App.css` - styling

## Frontend behavior

The UI communicates with the backend API using Axios and the following route base:

```text
/api/ToDoItems
```

Typical flow:

1. fetch tasks on load
2. render task list
3. update the local UI after create, edit, toggle, or delete
4. show errors if an API call fails

## Development notes

- The frontend expects the backend API to be running.
- If the backend uses HTTPS locally, your browser may ask you to trust the local development certificate.
- If the backend URL changes, update `REACT_APP_API_URL` instead of hardcoding a new URL in the source.
