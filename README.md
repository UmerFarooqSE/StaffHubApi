# StaffHubApi

![CI Pipeline](https://github.com/UmerFarooqSE/StaffHubApi/actions/workflows/ci.yml/badge.svg)

A .NET 9 Web API built with Docker, PostgreSQL, and Redis.

## Tech stack

- .NET 9 Minimal API and Controllers
- Entity Framework Core 9 with code-first migrations
- Dapper for complex reporting queries
- PostgreSQL 16
- Redis
- Docker and Docker Compose
- GitHub Actions CI/CD
- Azure App Service

## Architecture

- Repository pattern for Dapper queries
- EF Core DbContext for standard CRUD operations
- snake_case naming convention for PostgreSQL via EFCore.NamingConventions
- Global usings for clean file structure
- DTOs for Dapper result mapping

## CI/CD pipeline

Two branch strategy with full automation.

- `dev` branch: build, test, and format check on every push
- `main` branch: full pipeline including Docker Hub push and Azure deployment
- Branch protection: direct pushes to main blocked, CI must pass before merge
- Matrix builds: tested against .NET 8 and .NET 9 simultaneously
- NuGet caching for faster build times

## Running locally

```bash
docker compose up -d
```

API available at `http://localhost:7000`

## API endpoints

### Departments

| Method | Endpoint | Description | Data layer |
|--------|----------|-------------|------------|
| GET | /api/departments | Get all departments | EF Core |
| GET | /api/departments/{id} | Get department by ID | EF Core |
| POST | /api/departments | Create department | EF Core |
| PUT | /api/departments/{id} | Update department | EF Core |
| DELETE | /api/departments/{id} | Delete department | EF Core |

### Employees

| Method | Endpoint | Description | Data layer |
|--------|----------|-------------|------------|
| GET | /api/employees | Get all employees | EF Core |
| GET | /api/employees/{id} | Get employee by ID | EF Core |
| GET | /api/employees/summaries | Get employee summaries | Dapper |
| GET | /api/employees/{id}/detail | Get employee detail with colleague count | Dapper |
| POST | /api/employees | Create employee | EF Core |
| PUT | /api/employees/{id} | Update employee | EF Core |
| DELETE | /api/employees/{id} | Delete employee | EF Core |

### Health

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /healthz | API health status |

## Database

PostgreSQL 16 running in Docker with a persistent named volume.
Code-first migrations managed by EF Core.
snake_case table and column naming convention.

## Secrets management

All sensitive values stored as GitHub Secrets and Azure environment variables.
No credentials in source control or Docker images.