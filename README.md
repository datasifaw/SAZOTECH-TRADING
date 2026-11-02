# SAZOTECH-TRADING

**Trading Order Management System | C# .NET 10 | ASP.NET Core Web API**

![SAZOTECH Trading — Project Overview](image/INTERFACE_SAZOTECH_TRADING.png)

![SAZOTECH Trading — Target Interface](image/INTERFACE_2.png)

> **Project Status:** Phase 6 Completed — Automated Testing  
> **Current Version:** MVP under development  
> **Next Phase:** Phase 7 — Final Docker Setup & Documentation  
> **Trading Environment:** Simulated trading only

## Project Overview

SAZOTECH-TRADING is a backend-focused trading order management system built with C#, ASP.NET Core Web API, Entity Framework Core, and Microsoft SQL Server.

The application simulates financial instrument management, BUY and SELL order registration, order execution, portfolio position tracking, and basic portfolio valuation.

Automated tests validate core trading business rules using xUnit and isolated SQLite in-memory databases.

The project follows an incremental development workflow using Git feature branches, Conventional Commits, and GitHub Pull Requests.

All market prices and trading operations are simulated. No real financial transactions are performed.

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
| Isolated Test Database | SQLite in-memory |
| Security | .NET User Secrets, NuGet Vulnerability Auditing |
| Version Control | Git / GitHub |
| Development Environment | Visual Studio Code |

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
| 7 | Final Docker Setup & Documentation | Next |

## Implemented Features

### Phase 0 — Project Initialization

- Create the .NET solution and ASP.NET Core Web API project.
- Initialize Git and configure the GitHub repository.
- Configure `.gitignore` and project documentation.
- Establish a feature-branch development workflow.

### Phase 1 — REST API & Swagger

- Configure ASP.NET Core API controllers.
- Enable Swagger / OpenAPI.
- Add an application health-check endpoint.

### Phase 2 — Database Integration

- Run SQL Server 2022 in Docker.
- Configure Entity Framework Core 10.
- Implement the `TradingDbContext`.
- Create and apply Code First database migrations.
- Configure database credentials using .NET User Secrets.
- Add a database connectivity endpoint.

### Phase 3 — Financial Instruments CRUD

- Create financial instruments.
- Retrieve all instruments or an individual instrument.
- Update instrument information.
- Delete instruments when database constraints permit.
- Validate instrument properties.
- Prevent duplicate instrument symbols.

### Phase 4 — Trading Orders Management

- Register BUY and SELL orders.
- Associate orders with financial instruments.
- Validate order types and quantities.
- Calculate order totals using recorded prices.
- Persist trading orders in SQL Server.
- Retrieve orders individually or as a collection.
- Initialize new orders with `NEW` status.

### Phase 5 — Order Execution & Portfolio Positions

- Execute pending BUY and SELL orders.
- Update executed orders to `EXECUTED` status.
- Prevent sequential double execution.
- Reject SELL executions when holdings are insufficient.
- Create and update aggregated portfolio positions.
- Recalculate weighted average acquisition prices.
- Reduce positions after SELL executions.
- Remove positions when all shares are sold.
- Use SQL database transactions for order execution.
- Expose portfolio positions through REST endpoints.
- Calculate market value, cost basis, and unrealized P&L.

### Phase 6 — Automated Testing

- Create a dedicated `.NET 10` xUnit test project.
- Reference the API project from the test project.
- Add SQLite in-memory database support.
- Test portfolio position model properties.
- Test order-not-found handling.
- Test already-executed order protection.
- Test BUY position creation.
- Test weighted average acquisition prices.
- Test partial SELL executions.
- Test insufficient-holdings rejection.
- Test SELL orders without an existing position.
- Test complete position closure.
- Test invalid quantities, order types, and prices.
- Verify that rejected execution attempts do not alter positions.

**Latest validated test result: 16 passed, 0 failed.**

These tests cover the main application execution scenarios against SQLite in-memory databases. They do not yet replace SQL Server-specific integration or concurrent-execution testing.

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
| GET | `/api/orders` | Retrieve all trading orders |
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
3. Verify that the order status is `NEW`.
4. Execute the order.
5. Update the portfolio position and change the order status to `EXECUTED`.
6. Retrieve the updated portfolio positions.

### BUY Execution

A BUY increases the number of shares held.

When the instrument already has an open position, the weighted average acquisition price is recalculated:

**New Average Price = (Existing Quantity × Existing Average Price + Purchased Quantity × Purchase Price) / Total Quantity**

### SELL Execution

A SELL reduces the number of shares held.

An execution attempt returns a conflict when the available position is insufficient.

When all shares are sold, the open portfolio position is removed while the trading order history remains available.

### Order Statuses

| Status | Meaning |
|---|---|
| `NEW` | Registered but not yet executed |
| `EXECUTED` | Successfully executed |

Insufficient-holdings execution attempts are rejected with HTTP `409`, but the corresponding order remains `NEW` in the current MVP.

## Portfolio Valuation

| Indicator | Formula |
|---|---|
| Market Value | Quantity × Current Price |
| Cost Basis | Quantity × Average Acquisition Price |
| Unrealized P&L | Market Value − Cost Basis |

Calculations use simulated market data and exclude trading fees, taxes, and realized profit or loss.

## Database Architecture

**Database:** `SazotechTradingDb`

### Tables

| Table | Description |
|---|---|
| `FinancialInstruments` | Instrument reference data |
| `TradingOrders` | BUY and SELL order history |
| `PortfolioPositions` | Aggregated open positions |
| `__EFMigrationsHistory` | Entity Framework Core migration history |

### Relationships

```text
FinancialInstruments
       |
       +------< TradingOrders
       |
       +------- PortfolioPositions
```

Each financial instrument may be referenced by multiple trading orders and at most one aggregated portfolio position.

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
├── image/
│   ├── INTERFACE_SAZOTECH_TRADING.png
│   └── INTERFACE_2.png
├── README.md
└── docker-compose.yml
```

## Local Development Ports

| Service | Host | Port |
|---|---|---|
| ASP.NET Core Web API | localhost | 5187 |
| Swagger UI | localhost | 5187 |
| SQL Server Docker (Windows host) | 127.0.0.1 | 1435 |
| SQL Server (container) | Internal Docker network | 1433 |

**Important:** The Docker SQL Server instance uses internal port `1433`, mapped to host port `1435`, to avoid conflicts with existing local services.

Additional host-port reservations and conflict checks will be documented during Phase 7.

## Running the Project

### 1. Start SQL Server

From the repository root:

```powershell
docker compose up -d
docker compose ps
```

### 2. Restore Dependencies

```powershell
dotnet restore
dotnet tool restore
```

### 3. Build the Solution

```powershell
dotnet build
```

### 4. Configure Database Access

Configure `ConnectionStrings:TradingDb` through .NET User Secrets.

Never commit passwords, connection credentials, or local `.env` files.

### 5. Apply Database Migrations

```powershell
dotnet tool run dotnet-ef database update --project SAZOTECH.Trading.API --startup-project SAZOTECH.Trading.API
```

### 6. Start the API

```powershell
dotnet run --project SAZOTECH.Trading.API --launch-profile http
```

### 7. Open Swagger UI

http://localhost:5187/swagger

## Automated Testing

The test project uses **xUnit** with **SQLite in-memory databases** to validate trading behavior without modifying the SQL Server development database.

### Run All Tests

```powershell
dotnet test
```

### Run Only the Test Project

```powershell
dotnet test SAZOTECH.Trading.Tests/SAZOTECH.Trading.Tests.csproj
```

### Latest Validated Results

| Metric | Result |
|---|---:|
| Total tests | 16 |
| Passed | 16 |
| Failed | 0 |
| Skipped | 0 |

### Test Coverage

- Portfolio position model properties
- Missing trading orders
- Previously executed orders
- Initial BUY position creation
- Weighted average price calculation
- Partial SELL execution
- Insufficient portfolio quantity
- SELL without an open position
- Complete position closure
- Invalid order type, quantity, and price
- Prevention of sequential double execution

### Test Limitations

SQLite tests are isolated and fast, but database behavior may differ from SQL Server.

Concurrent execution, SQL Server-specific transactions and locking, and full HTTP integration tests remain future improvements.

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

The endpoint updates the order and portfolio position on successful execution.

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

The example uses simulated values.

## Git Workflow

The project follows feature-branch development, Conventional Commits, and GitHub Pull Requests.

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

Each major development phase is reviewed and merged into `main`.

## Next Phase — Final Docker Setup & Documentation

Phase 7 will focus on finalizing local deployment and the project's documentation.

Planned tasks include:

- Review Docker Compose configuration.
- Validate SQL Server startup and persistence.
- Review application configuration and secrets handling.
- Verify build and test commands from a clean environment.
- Document required tools and setup instructions.
- Review local port reservations and potential conflicts.
- Finalize the project README and deployment instructions.

## Future Improvements

- SQL Server integration testing
- Concurrent-order execution tests
- Additional trading order lifecycle states
- Realized P&L calculations
- Authentication and authorization
- Multiple trading accounts and portfolios
- Improved logging and observability
- Custom trading dashboard

## Project Interface Preview

The images shown at the beginning of this README are conceptual interface mockups.

The current MVP provides a backend REST API with Swagger documentation. A custom web trading dashboard has not yet been implemented.

## Disclaimer

SAZOTECH-TRADING is an educational and portfolio project.

All market data and trading operations are simulated. The application does not connect to live financial exchanges, provide investment advice, or perform real financial transactions.