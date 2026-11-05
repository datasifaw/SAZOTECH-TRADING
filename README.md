# SAZOTECH-TRADING ?
   
**Trading Order Management System | C# .NET 10 | ASP.NET Core Web API**

![SAZOTECH Trading — Project Overview](image/INTERFACE_SAZOTECH_TRADING.png)

![SAZOTECH Trading — Target Interface](image/INTERFACE_2.png)

> **Project Status:** MVP v1.0.0 Released  
> **Release:** v1.0.0 — Published on GitHub  
> **Development Progress:** Phases 0–7 Completed  
> **Automated Tests:** 16 Passed, 0 Failed  
> **Trading Environment:** Simulated trading only

## Project Overview

SAZOTECH-TRADING is a backend-focused trading order management system developed using C#, .NET 10, ASP.NET Core Web API, Entity Framework Core 10, and Microsoft SQL Server 2022.

The application supports financial instrument management, BUY and SELL order registration, simulated order execution, portfolio position tracking, and basic portfolio valuation.

The project was developed incrementally using Git feature branches, Conventional Commits, and GitHub Pull Requests.

**Version 1.0.0 marks the completion of the initial backend MVP roadmap.**

No real market trading or financial transactions are performed.

## Technology Stack

| Category | Technology |
|---|---|
| Backend | C#, .NET 10, ASP.NET Core Web API |
| API Documentation | Swagger / OpenAPI |
| Database | Microsoft SQL Server 2022 |
| ORM | Entity Framework Core 10 |
| Database Migrations | EF Core Code First |
| Containers | Docker / Docker Compose |
| Database Management | SQL Server Management Studio (SSMS) |
| Automated Testing | xUnit |
| Test Database | SQLite in-memory |
| Security | .NET User Secrets, environment variables |
| Development Environment | Visual Studio Code |
| Version Control | Git / GitHub |

## Development Progress

| Phase | Description | Status |
|---|---|---|
| 0 | Project Initialization & Git Setup | Completed |
| 1 | REST API & Swagger Setup | Completed |
| 2 | SQL Server, Docker & EF Core Integration | Completed |
| 3 | Financial Instruments CRUD API | Completed |
| 4 | Trading Orders Management | Completed |
| 5 | Order Execution & Portfolio Positions | Completed |
| 6 | Automated Testing with xUnit | Completed |
| 7 | Final Docker Setup & Documentation | Completed |

**All eight development phases are complete.**

## Implemented Features

### Phase 0 — Project Initialization

- Created the .NET solution and ASP.NET Core Web API project.
- Initialized Git and configured GitHub.
- Configured `.gitignore` and project documentation.
- Established a feature-branch development workflow.

### Phase 1 — REST API & Swagger

- Configured ASP.NET Core API controllers.
- Enabled Swagger / OpenAPI documentation.
- Added an application health-check endpoint.

### Phase 2 — SQL Server & EF Core Integration

- Configured SQL Server 2022 using Docker Compose.
- Integrated Entity Framework Core 10.
- Implemented `TradingDbContext`.
- Created and applied Code First migrations.
- Secured database credentials using .NET User Secrets.
- Added a database connectivity endpoint.

### Phase 3 — Financial Instruments CRUD

- Created financial instruments.
- Retrieved all instruments or one instrument by ID.
- Updated instrument information.
- Deleted instruments when database constraints permit.
- Validated instrument properties.
- Prevented duplicate instrument symbols.

### Phase 4 — Trading Orders Management

- Registered BUY and SELL orders.
- Associated trading orders with financial instruments.
- Validated order types and quantities.
- Calculated order amounts from recorded prices.
- Persisted orders in SQL Server.
- Retrieved orders individually or as a collection.
- Initialized orders with the `NEW` status.

### Phase 5 — Order Execution & Portfolio Positions

- Executed pending BUY and SELL orders.
- Updated successful orders to `EXECUTED`.
- Prevented sequential double execution.
- Rejected SELL execution when holdings were insufficient.
- Created and updated aggregated portfolio positions.
- Calculated weighted average acquisition prices.
- Reduced positions after SELL execution.
- Removed fully closed positions.
- Used SQL transactions for order and position updates.
- Exposed portfolio retrieval endpoints.
- Calculated market value, cost basis, and unrealized P&L.

### Phase 6 — Automated Testing

- Created a dedicated xUnit test project targeting .NET 10.
- Added SQLite in-memory databases for isolated tests.
- Tested portfolio position models.
- Tested BUY creation and weighted average price calculations.
- Tested partial and full SELL execution.
- Tested insufficient holdings and missing positions.
- Tested invalid orders and HTTP error responses.
- Verified protection against sequential double execution.

**Validated result: 16 automated tests passed, 0 failed.**

### Phase 7 — Final Docker Setup & Documentation

- Validated Docker Compose configuration.
- Verified SQL Server connectivity and database storage.
- Confirmed the local SQL Server port mapping.
- Protected `.env` from version control.
- Added a safe `.env.example` template.
- Included the xUnit test project in the `.slnx` solution.
- Documented local ports in `docs/PORTS.md`.
- Completed the MVP setup and API documentation.
- Built and tested the project in Release configuration.
- Published Git tag `v1.0.0` and the GitHub Release.

## API Endpoints

### System

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/health` | Application health check |
| GET | `/api/database/status` | SQL Server connectivity check |

### Financial Instruments

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/instruments` | Retrieve all instruments |
| GET | `/api/instruments/{id}` | Retrieve an instrument by ID |
| POST | `/api/instruments` | Create an instrument |
| PUT | `/api/instruments/{id}` | Update an instrument |
| DELETE | `/api/instruments/{id}` | Delete an instrument |

### Trading Orders

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/orders` | Retrieve all orders |
| GET | `/api/orders/{id}` | Retrieve an order by ID |
| POST | `/api/orders` | Create a BUY or SELL order |
| POST | `/api/orders/{id}/execute` | Execute a pending trading order |

### Portfolio Positions

| Method | Endpoint | Description |
|---|---|---|
| GET | `/api/positions` | Retrieve all open positions |
| GET | `/api/positions/{id}` | Retrieve a position by ID |

## Trading Workflow

1. Register or retrieve a financial instrument.
2. Create a BUY or SELL order.
3. Confirm that the order status is `NEW`.
4. Execute the trading order.
5. Update portfolio holdings and mark the order as `EXECUTED`.
6. Retrieve updated portfolio positions.

### BUY Execution

A BUY increases the number of shares held.

If a position already exists, its weighted average acquisition price is recalculated:

**New Average Price = (Existing Quantity × Existing Average Price + Purchased Quantity × Purchase Price) / Total Quantity**

### SELL Execution

A SELL reduces the number of shares held.

Execution is refused when available holdings are insufficient. A position is removed when its remaining quantity reaches zero, while trading order history is preserved.

### Order Statuses

| Status | Meaning |
|---|---|
| `NEW` | Registered but not yet executed |
| `EXECUTED` | Successfully executed |

An insufficient-holdings execution attempt returns HTTP `409`, while the order remains `NEW`.

## Portfolio Valuation

| Indicator | Formula |
|---|---|
| Market Value | Quantity × Current Price |
| Cost Basis | Quantity × Average Acquisition Price |
| Unrealized P&L | Market Value − Cost Basis |

Calculations use simulated market prices and exclude taxes, fees, and realized P&L.

## Database Architecture

**Database Name:** `SazotechTradingDb`

| Table | Description |
|---|---|
| `FinancialInstruments` | Financial instrument reference data |
| `TradingOrders` | BUY and SELL trading order history |
| `PortfolioPositions` | Current aggregated portfolio holdings |
| `__EFMigrationsHistory` | EF Core migration history |

### Relationships

```text
FinancialInstruments
       |
       +------< TradingOrders
       |
       +------- PortfolioPositions
```

One financial instrument can have multiple trading orders and at most one aggregated portfolio position.

## Project Structure

```text
SAZOTECH-TRADING/
├── SAZOTECH.Trading.API/
│   ├── Controllers/
│   ├── Data/
│   ├── Migrations/
│   ├── Models/
│   └── Program.cs
├── SAZOTECH.Trading.Tests/
│   ├── Controllers/
│   │   └── OrderExecutionsControllerTests.cs
│   ├── Models/
│   │   └── PortfolioPositionTests.cs
│   └── SAZOTECH.Trading.Tests.csproj
├── docs/
│   └── PORTS.md
├── image/
│   ├── INTERFACE_SAZOTECH_TRADING.png
│   └── INTERFACE_2.png
├── .env.example
├── README.md
├── docker-compose.yml
└── SAZOTECH-TRADING.slnx
```

## Local Development Ports

| Service | Host | Port |
|---|---|---|
| ASP.NET Core Web API | localhost | 5187 |
| Swagger UI | localhost | 5187 |
| SQL Server Docker | 127.0.0.1 | 1435 |
| SQL Server Container | Internal Docker network | 1433 |

**Important:** Windows host port `1435` forwards connections to SQL Server's internal Docker port `1433`.

### Complete Port Reference

See **[Local Port Reference](docs/PORTS.md)** for all reserved development ports, Docker port mappings, and troubleshooting commands.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Docker Desktop with Docker Compose
- Git
- Visual Studio Code or another compatible editor
- SQL Server Management Studio (optional)

### 1. Configure the Docker Environment

Copy `.env.example` to `.env` and replace the placeholder with a strong SQL Server password.

The actual `.env` file must not be committed to Git.

### 2. Start SQL Server

```powershell
docker compose up -d
docker compose ps
```

### 3. Restore Dependencies

```powershell
dotnet restore
dotnet tool restore
```

### 4. Configure the Connection String

Configure `ConnectionStrings:TradingDb` using .NET User Secrets in the API project.

The local database connection must target `127.0.0.1,1435`.

Keep database passwords out of committed configuration files.

### 5. Apply Database Migrations

```powershell
dotnet tool run dotnet-ef database update --project SAZOTECH.Trading.API --startup-project SAZOTECH.Trading.API
```

### 6. Build the Project

```powershell
dotnet build --configuration Release
```

### 7. Run Automated Tests

```powershell
dotnet test SAZOTECH.Trading.Tests/SAZOTECH.Trading.Tests.csproj --configuration Release
```

### 8. Start the API

```powershell
dotnet run --project SAZOTECH.Trading.API --launch-profile http
```

### 9. Open Swagger UI

http://localhost:5187/swagger

### 10. Check Database Connectivity

Execute `GET /api/database/status` in Swagger.

A successful database connection should return HTTP `200`.

## Automated Testing Results

| Metric | Validated Result |
|---|---:|
| Total Tests | 16 |
| Passed | 16 |
| Failed | 0 |
| Skipped | 0 |

Tests use xUnit and SQLite in-memory databases to verify trading rules without modifying the SQL Server development database.

### Test Coverage

- Portfolio position model
- Missing and already executed orders
- Initial BUY position creation
- Weighted average acquisition prices
- Partial SELL execution
- Insufficient holdings rejection
- SELL without an open position
- Full position closure
- Invalid order types, quantities, and prices
- Sequential double-execution prevention

### Testing Limitations

SQLite tests do not replace SQL Server-specific integration tests.

Concurrent-order execution and SQL Server locking behavior require additional testing.

## API Usage Examples

### Create a BUY Order

**POST** `/api/orders`

```json
{
  "financialInstrumentId": 1,
  "orderType": "BUY",
  "quantity": 10
}
```

### Execute an Order

**POST** `/api/orders/1/execute`

A successful execution updates both the trading order and portfolio position.

### Retrieve Portfolio Positions

**GET** `/api/positions`

Illustrative response:

```json
[
  {
    "id": 1,
    "financialInstrumentId": 1,
    "symbol": "AAPL",
    "name": "Apple Inc.",
    "quantity": 10,
    "averagePrice": 225.75,
    "currentPrice": 230.00,
    "marketValue": 2300.00,
    "costBasis": 2257.50,
    "unrealizedPnL": 42.50,
    "updatedAt": "2026-11-02T10:00:00Z"
  }
]
```

All values and identifiers are illustrative.

## Git Workflow

Development follows feature branches, Conventional Commits, and GitHub Pull Requests.

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

### Official Release

**Version:** `v1.0.0`  
**Release Name:** SAZOTECH-TRADING v1.0.0 — MVP Release  
**Release Status:** Published on GitHub

Version `v1.0.0` represents the completed backend MVP, covering development phases 0 through 7.

## Known Limitations

- Single aggregated simulated portfolio
- No multi-account trading support
- No live market data or exchange connections
- No real financial transactions
- No realized P&L calculations
- No custom web dashboard
- No automated SQL Server integration tests
- No comprehensive concurrent-execution tests

## Future Improvements

- SQL Server integration testing
- Concurrency and transaction stress testing
- Additional order lifecycle states
- Authentication and authorization
- Multi-account and multi-portfolio support
- Realized profit and loss calculations
- Centralized logging and observability
- Fully containerized API deployment
- Custom trading dashboard

## Project Interface Preview

The images displayed at the beginning of this README are conceptual interface mockups.

The current MVP exposes a backend REST API documented through Swagger UI. A custom trading dashboard has not yet been implemented.

## Disclaimer

SAZOTECH-TRADING is an educational and portfolio project.

All market prices and trading operations are simulated. The application does not connect to live financial exchanges, provide investment advice, or perform real financial transactions.
