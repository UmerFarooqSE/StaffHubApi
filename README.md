# StaffHubApi

![CI Pipeline](https://github.com/UmerFarooqSE/StaffHubApi/actions/workflows/ci.yml/badge.svg)

A .NET 9 Web API built with Docker, PostgreSQL, and Redis, developed as part of a 25-day senior developer upskilling program.

## Tech stack

- .NET 9
- Docker and Docker Compose
- PostgreSQL
- Redis
- GitHub Actions CI/CD

## Running locally

```bash
docker compose up -d
```

API available at `http://localhost:7000`

## Health check

```
GET /healthz
```