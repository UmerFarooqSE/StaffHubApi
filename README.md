# StaffHubApi

![CI Pipeline](https://github.com/UmerFarooqSE/StaffHubApi/actions/workflows/ci.yml/badge.svg)

A .NET 9 Web API built with Docker, PostgreSQL, and Redis.

## Tech stack

- .NET 9 Minimal API
- Docker and Docker Compose
- PostgreSQL
- Redis
- GitHub Actions CI/CD
- Azure App Service

## CI/CD pipeline

This project uses a professional two-branch CI/CD strategy.

### Dev branch pipeline

Every push to `dev` triggers the CI Dev Branch workflow which runs the following steps:

1. Checkout code from the repository
2. Setup .NET 8 and .NET 9 in parallel using matrix builds
3. Restore NuGet packages using cached layers for faster builds
4. Build the application
5. Check code formatting using dotnet format
6. Run all unit tests

No deployment happens on dev pushes. The dev branch is for verification only.

### Main branch pipeline

Every merge to `main` triggers the full CI/CD pipeline:

1. Reusable build and test job runs first, same steps as dev
2. On success, the deploy job starts automatically
3. Logs in to Docker Hub using GitHub Secrets
4. Builds a Docker image tagged with the Git commit SHA
5. Pushes the image to Docker Hub for versioned artifact storage
6. Publishes the .NET application in Release mode
7. Authenticates with Azure using a Service Principal via GitHub Secrets
8. Deploys to Azure App Service

Deployment is blocked automatically if any build, format, or test step fails.

### Branch protection

The main branch is protected with the following rules:

- Direct pushes to main are blocked, all changes must come through a pull request
- The CI build job must pass before any pull request can be merged
- This prevents broken or unformatted code from ever reaching production

## Running locally

```bash
docker compose up -d
```

API available at `http://localhost:7000`

## Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /weatherforecast | Returns 5 days of weather data |
| GET | /healthz | Returns API health status |

## Secrets and configuration

All sensitive values are stored as GitHub Secrets and Azure Key Vault references. No credentials exist in the codebase or pipeline files.