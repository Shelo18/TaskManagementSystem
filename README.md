# Task Management System

ASP.NET Core 8 Web API project for managing projects, tasks, users and comments.

## Technologies

- ASP.NET Core 8
- Entity Framework Core 8 + SQL Server
- FluentValidation
- AutoMapper
- xUnit + Moq
- Swagger

## How to run

1. Clone the repo
2. Open `TaskManagement.sln` in Visual Studio
3. Update connection string in `appsettings.json` if needed
4. Run the project — migrations apply automatically on startup
5. Swagger opens in the browser at the root URL

## Running tests

Open Test Explorer in Visual Studio and run all tests.

## Database

Migrations are in `TaskManagement.Infrastructure/Migrations/`. There is also a manual SQL script `database_setup.sql` in the root if you prefer to set up the database manually via SSMS.

## Project structure

- `TaskManagement.Core` — entities, DTOs, interfaces, validators
- `TaskManagement.Infrastructure` — DbContext, repositories, services, AutoMapper profile
- `TaskManagement.API` — controllers, middleware, program entry point
- `TaskManagement.Tests` — unit tests for all services
