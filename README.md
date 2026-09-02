# ProductionReadyApi

[![CI](https://github.com/dennismorina/ProductionReadyApi/actions/workflows/ci.yml/badge.svg)](https://github.com/dennismorina/ProductionReadyApi/actions/workflows/ci.yml)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-336791)
![Docker](https://img.shields.io/badge/Docker-ready-2496ED)
![License](https://img.shields.io/badge/License-MIT-green)

A production-oriented ASP.NET Core REST API demonstrating clean backend architecture,
PostgreSQL persistence, automated testing, containerization and continuous integration.

The project intentionally uses a small product-management domain so the focus stays on
engineering practices rather than artificial business complexity.

## Highlights

- ASP.NET Core / .NET 10
- C#
- RESTful Product API
- Layered, Clean Architecture-inspired structure
- PostgreSQL 17
- Entity Framework Core
- EF Core migrations
- Repository abstraction
- Application services
- Domain validation
- Centralized exception handling with Problem Details
- Search and pagination
- Unique SKU enforcement
- Health checks
- OpenAPI
- Scalar API documentation in Development
- Docker and Docker Compose
- Unit tests
- Integration tests
- Code coverage in CI
- GitHub Actions
- Docker image build validation
- Dependabot

## Architecture

```text
┌───────────────────────────────────┐
│                API                │
│ Controllers · HTTP · OpenAPI      │
└────────────────┬──────────────────┘
                 │
                 ▼
┌───────────────────────────────────┐
│            Application            │
│ Services · Contracts · DTOs       │
└────────────────┬──────────────────┘
                 │
                 ▼
┌───────────────────────────────────┐
│              Domain               │
│ Entities · Business Rules         │
└───────────────────────────────────┘

                 ▲
                 │ IProductRepository
                 │
┌────────────────┴──────────────────┐
│          Infrastructure           │
│ EF Core · PostgreSQL · Migrations │
└───────────────────────────────────┘
```

The domain and application layers are kept independent from PostgreSQL and Entity
Framework Core. Infrastructure implements persistence concerns, while the API exposes
the application through HTTP.

## Project Structure

```text
ProductionReadyApi
├── src
│   ├── ProductionReadyApi.Api
│   ├── ProductionReadyApi.Application
│   ├── ProductionReadyApi.Domain
│   └── ProductionReadyApi.Infrastructure
├── tests
│   ├── ProductionReadyApi.UnitTests
│   └── ProductionReadyApi.IntegrationTests
├── .github
│   ├── workflows
│   │   └── ci.yml
│   └── dependabot.yml
├── docker-compose.yml
├── ProductionReadyApi.sln
└── README.md
```

## API

### Products

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/products` | Returns paginated products |
| `GET` | `/api/products/{id}` | Returns one product |
| `POST` | `/api/products` | Creates a product |
| `PUT` | `/api/products/{id}` | Updates a product |
| `DELETE` | `/api/products/{id}` | Deletes a product |

### Health Checks

| Endpoint | Purpose |
|---|---|
| `/health` | Overall application health |
| `/health/live` | Liveness probe |
| `/health/ready` | Readiness probe including database connectivity |

## Example Request

```http
POST /api/products
Content-Type: application/json

{
  "sku": "KB-001",
  "name": "Mechanical Keyboard",
  "price": 129.90,
  "stockQuantity": 25
}
```

The API normalizes SKUs and prevents duplicate values. Invalid input returns structured
HTTP error responses instead of leaking persistence-specific exceptions.

## Search and Pagination

```http
GET /api/products?search=keyboard&page=1&pageSize=20
```

Pagination prevents unrestricted result sets and represents a common production backend
pattern.

## Error Handling

The API uses centralized exception handling and standardized Problem Details responses.

Typical status codes include:

- `400 Bad Request`
- `404 Not Found`
- `409 Conflict`
- `500 Internal Server Error`

For example, trying to create two products with the same SKU returns `409 Conflict`.

## Database

PostgreSQL is used as the relational database and Entity Framework Core handles
persistence and migrations.

The initial migration creates the `products` table and a unique index for the SKU.

Applied EF Core migrations are tracked in:

```text
__EFMigrationsHistory
```

## Run Locally

### Requirements

- .NET 10 SDK
- Docker Desktop

Start PostgreSQL:

```bash
docker compose up -d postgres
```

Run the API:

```bash
dotnet run --project src/ProductionReadyApi.Api
```

Default local development endpoints:

```text
HTTP:       http://localhost:5080
HTTPS:      https://localhost:7080
PostgreSQL: localhost:5433
```

## API Documentation

When the application runs in the `Development` environment, interactive Scalar
documentation is available at:

```text
http://localhost:5080/scalar/v1
```

The OpenAPI document is available at:

```text
http://localhost:5080/openapi/v1.json
```

## Run with Docker

Build and start the complete environment:

```bash
docker compose up --build -d
```

The environment contains:

```text
Docker Compose
├── API
│   └── localhost:8080
└── PostgreSQL
    ├── container:5432
    └── host:5433
```

Check readiness:

```text
http://localhost:8080/health/ready
```

Stop the environment:

```bash
docker compose down
```

To also remove the PostgreSQL volume:

```bash
docker compose down -v
```

> Warning: `-v` removes the local database data stored in the Docker volume.

## Testing

Run all tests:

```bash
dotnet test --solution ProductionReadyApi.sln --configuration Release
```

The solution currently contains unit and integration tests. Integration tests execute
requests through the ASP.NET Core HTTP pipeline.

## Continuous Integration

Every push and pull request targeting `main` runs the GitHub Actions CI workflow:

```text
Restore
   ↓
Release Build
   ↓
Unit & Integration Tests
   ↓
Code Coverage
   ↓
Docker Image Build
```

A change must compile, pass the automated tests and produce a valid Docker image before
the workflow succeeds.

## Dependency Management

Dependabot checks dependencies regularly and can create pull requests for updates to:

- NuGet packages
- GitHub Actions
- Docker images
- Docker Compose images
- .NET SDK versions

Dependabot pull requests run through the same CI checks as normal changes.

## Technology Stack

| Area | Technology |
|---|---|
| Language | C# |
| Runtime | .NET 10 |
| API | ASP.NET Core |
| Database | PostgreSQL 17 |
| ORM | Entity Framework Core |
| API Documentation | OpenAPI / Scalar |
| Testing | xUnit |
| Coverage | Coverlet |
| Containers | Docker / Docker Compose |
| CI | GitHub Actions |
| Dependency Updates | Dependabot |

## Design Goals

This repository demonstrates several practices used in maintainable backend systems:

- clear separation of concerns
- dependency inversion
- persistence abstraction
- explicit domain rules
- standardized API errors
- database migrations
- automated testing
- reproducible local environments
- continuous integration
- automated dependency maintenance

The project deliberately stays small enough that its architecture can be understood
without navigating a large artificial domain.

## License

This project is licensed under the MIT License.
