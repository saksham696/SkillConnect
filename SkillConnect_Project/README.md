# SkillConnect — Job Portal for Elevate Workforce Solutions

A full-stack job portal built for Code Art Web Technologies' client, Elevate
Workforce Solutions. Backend: ASP.NET Core Web API (C#, MVC + Repository
pattern) with SQL Server via Entity Framework Core. Frontend: React +
TypeScript SPA (Vite, Tailwind CSS, shadcn/ui).

See `SkillConnect_Project_Documentation.docx` (in the parent folder of this
zip, or alongside it) for the full write-up: problem statement, proposed
solution, features, architecture, ER diagram, UML class/use-case/sequence
diagrams, MVC mapping, OOP principles applied, and the API reference.

## Project Structure

```
SkillConnect.Api/          ASP.NET Core Web API (backend)
  Controllers/              UserController, JobController, JobApplicationController
  Entities/                 EF Core entities: User, UserProfile, Job, JobApplication
  Models/                   DTOs: request/response shapes + PagedResult<T>
  IServices/                Repository interfaces (abstractions)
  Services/                 Repository implementations (business logic + data access)
  Helpers/                  JwtTokenHelper, CurrentUserHelper, FileUploadHelper
  Migrations/                EF Core migrations
  Program.cs                 App startup, DI registrations, JWT + CORS config

skillconnect-frontend/     React + TypeScript SPA (frontend)
  src/pages/                 LandingPage, LoginPage, Register, DashboardPage (Company)
  src/components/            Navbar, Pagination, JobDetailModal, JobFormModal, ui/*
  src/context/                AuthContext (JWT session state, persisted in localStorage)
  src/lib/                    axios clients (public/private)
  src/types/                  Shared TypeScript types matching backend DTOs

docker-compose.yml         SQL Server container for local development
```

## Running the Backend

Requirements: .NET 10 SDK, SQL Server (or use `docker-compose up` to start one
locally).

```bash
cd SkillConnect.Api
dotnet restore
dotnet ef database update   # applies the Migrations/ folder to create the schema
dotnet run
```

The API listens on the port configured in
`Properties/launchSettings.json` (defaults used by the frontend: `http://localhost:5129`).

## Running the Frontend

Requirements: Node.js 20+.

```bash
cd skillconnect-frontend
npm install
npm run dev
```

Vite serves the app on `http://localhost:5173` by default. The API base URL
is configured in `src/lib/axios.ts`.

## Test Accounts

There are no seeded accounts — register through the UI. When registering,
choose **Company** to post jobs and review applicants, or **Job Seeker** to
browse and apply to jobs.

## Notes

- Passwords are hashed with BCrypt; JWTs carry the user's id, email, name,
  and role (`Company` / `JobSeeker`) as claims.
- Job ownership is enforced server-side: a company can only edit/delete jobs
  it posted, and only see applicants for its own postings.
- Uploaded resumes are stored under `SkillConnect.Api/uploads/` with
  GUID-prefixed filenames.
