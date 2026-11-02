# SAZOTECH-TRADING

**Trading Order Management System | C# .NET 10 | ASP.NET Core Web API**

![SAZOTECH Trading — Project Overview](image/INTERFACE_SAZOTECH_TRADING.png)

![SAZOTECH Trading — Target Interface](image/INTERFACE_2.png)

> **Project Status:** Phase 4 Completed — Trading Orders Management  
> **Current Version:** MVP under development  
> **Next Phase:** Order Execution & Portfolio Positions  
> **Trading Environment:** Simulated trading only

## Project Overview

SAZOTECH-TRADING is a backend-focused trading order management system built using C#, ASP.NET Core Web API, Entity Framework Core, and SQL Server.

The application simulates financial instrument management and BUY/SELL order registration, with order execution and portfolio position tracking planned for the next development phase.

The project follows an incremental development workflow using Git feature branches, Conventional Commits, and GitHub Pull Requests.

All trading activities are simulated. No real market transactions are executed.

## Technology Stack

- **Backend:** C#, .NET 10, ASP.NET Core Web API
- **API Documentation:** Swagger / OpenAPI
- **Database:** Microsoft SQL Server 2022
- **ORM:** Entity Framework Core 10
- **Database Migrations:** EF Core Code First
- **Containers:** Docker / Docker Compose
- **Database Management:** SQL Server Management Studio (SSMS)
- **Security:** .NET User Secrets, NuGet Vulnerability Auditing
- **Version Control:** Git / GitHub
- **Development Environment:** Visual Studio Code

## Development Progress

| Phase | Description | Status |
|---|---|---|
| 0 | Project Initialization & Git Setup | Completed |
| 1 | REST API & Swagger Setup | Completed |
| 2 | SQL Server, Docker & EF Core Integration | Completed |
| 3 | Financial Instruments CRUD API | Completed |
| 4 | Trading Orders Management | Completed |
| 5 | Order Execution & Portfolio Positions | Next |
| 6 | Unit Testing | Planned |
| 7 | Final Docker Setup & Documentation | Planned |

## Implemented Features

### API Foundation
- ASP.NET Core REST API setup
- Swagger / OpenAPI integration
- Application health check
- Database connectivity health check

### Database Integration
- SQL Server 2022 running in Docker
- Entity Framework Core 10 integration
- Code First database migrations
- Financial instrument and trading order entities
- One-to-many instrument-to-order relationship
- Secure local database configuration

### Financial Instruments Management
- Retrieve all financial instruments
- Retrieve instrument by ID
- Create financial instruments
- Update instrument information
- Delete financial instruments
- Validate instrument data
- Prevent duplicate symbols through application checks and a database unique index

### Trading Orders Management
- Create BUY orders
- Create SELL orders
- Retrieve all trading orders
- Retrieve trading order by ID
- Validate order type and quantity
- Calculate simulated order amount
- Link orders to financial instruments
- Store trading orders in SQL Server
- Initialize orders with `NEW` status

**Note:** Order execution, position updates, and sell-side position checks are not yet implemented.

## API Endpoints

### System

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/health` | Application health status |
| GET | `/api/database/status` | SQL Server connectivity status |

### Financial Instruments

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/instruments` | Retrieve all instruments |
| GET | `/api/instruments/{id}` | Retrieve instrument by ID |
| POST | `/api/instruments` | Create an instrument |
| PUT | `/api/instruments/{id}` | Update an instrument |
| DELETE | `/api/instruments/{id}` | Delete an instrument |

### Trading Orders

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/orders` | Retrieve all trading orders |
| GET | `/api/orders/{id}` | Retrieve a trading order by ID |
| POST | `/api/orders` | Create a BUY or SELL order |

## Database

**Database Name:** `SazotechTradingDb`

### Database Tables

| Table | Description |
|---|---|
| `FinancialInstruments` | Financial instrument reference data |
| `TradingOrders` | BUY and SELL orders |
| `__EFMigrationsHistory` | EF Core migration history |

### Database Relationships

```text
FinancialInstruments
        |
        | 1:N
        |
        v
    TradingOrders
```

Each trading order references an existing financial instrument using `FinancialInstrumentId`.

## Local Development Configuration

| Service | Host | Port |
|---|---|---|
| ASP.NET Core API | localhost | 5187 |
| Swagger UI | localhost | 5187 |
| SQL Server Docker | 127.0.0.1 | 1435 |
| SQL Server Container | Internal | 1433 |

Database credentials are stored locally and excluded from Git.

## Getting Started

### 1. Start SQL Server

```bash
docker compose up -d
docker compose ps
```

### 2. Restore Dependencies

```bash
dotnet restore
dotnet tool restore
```

### 3. Build the Application

```bash
dotnet build
```

### 4. Configure Database Credentials

Create a local `.env` file for Docker Compose based on `.env.example` if available.

Configure `ConnectionStrings:TradingDb` using .NET User Secrets.

Never commit credentials or secret configuration files.

### 5. Apply Database Migrations

```bash
dotnet tool run dotnet-ef database update --project SAZOTECH.Trading.API --startup-project SAZOTECH.Trading.API
```

### 6. Start the API

```bash
dotnet run --project SAZOTECH.Trading.API --launch-profile http
```

### 7. Open Swagger UI

http://localhost:5187/swagger

## Example — Create a Trading Order

**POST** `/api/orders`

```json
{
  "financialInstrumentId": 1,
  "orderType": "BUY",
  "quantity": 10
}
```

Example response:

```json
{
  "id": 1,
  "financialInstrumentId": 1,
  "symbol": "AAPL",
  "orderType": "BUY",
  "quantity": 10,
  "price": 225.75,
  "totalAmount": 2257.50,
  "status": "NEW",
  "createdAt": "2026-10-30T12:00:00Z"
}
```

The values shown above are illustrative and may differ from the database.

## Git Workflow

Each major development phase uses a dedicated feature branch.

```text
main
 ├── feature/project-setup
 ├── feature/database
 ├── feature/instruments
 ├── feature/trading-orders
 ├── feature/positions
 ├── feature/unit-tests
 └── feature/docker
```

Changes are committed using Conventional Commits and integrated through GitHub Pull Requests.

## Next Phase — Order Execution & Portfolio Positions

Planned capabilities:

- Execute pending trading orders
- Transition order statuses from `NEW` to `EXECUTED` or `REJECTED`
- Track portfolio positions
- Update quantities following BUY and SELL executions
- Reject sell executions with insufficient positions
- Preserve trade execution history
- Calculate basic position valuation and simulated P&L

## Future Improvements

- Unit and integration tests
- Business rule and risk validation
- Improved API error handling
- DTO-based request and response models
- Authentication and authorization
- Containerized application deployment
- Improved API documentation and logging

## Project Interface Preview

The images at the top of this README represent conceptual UI mockups. The current MVP exposes a REST API through Swagger UI; a custom trading dashboard has not yet been implemented.

## Disclaimer

SAZOTECH-TRADING is an educational and portfolio project. Market prices and trading operations are simulated. The system does not connect to live exchanges or perform real financial transactions.
