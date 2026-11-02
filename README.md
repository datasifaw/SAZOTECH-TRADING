# SAZOTECH-TRADING

**Trading Order Management System | C# .NET 10 | ASP.NET Core Web API**

![SAZOTECH Trading — Project Overview](image/INTERFACE_SAZOTECH_TRADING.png)

![SAZOTECH Trading — Target Interface](image/INTERFACE_2.png)

> **Project Status:** Phase 5 Completed — Order Execution & Portfolio Positions  
> **Current Version:** MVP under development  
> **Next Phase:** Unit Testing  
> **Trading Environment:** Simulated trading only

## Project Overview

SAZOTECH-TRADING is a backend-focused trading order management system developed using C#, ASP.NET Core Web API, Entity Framework Core, and Microsoft SQL Server.

The application simulates financial instrument management, BUY and SELL order registration, order execution, portfolio position tracking, and basic portfolio valuation.

The project follows an incremental development workflow using Git feature branches, Conventional Commits, and GitHub Pull Requests.

All market data and trading operations are simulated. No real financial transactions are executed.

## Technology Stack

- **Backend:** C#, .NET 10, ASP.NET Core Web API
- **API Documentation:** Swagger / OpenAPI
- **Database:** Microsoft SQL Server 2022
- **ORM:** Entity Framework Core 10
- **Migrations:** EF Core Code First
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
| 5 | Order Execution & Portfolio Positions | Completed |
| 6 | Unit Testing | Next |
| 7 | Final Docker Setup & Documentation | Planned |

## Implemented Features

### Phase 0 — Project Initialization

- Create the .NET solution and ASP.NET Core Web API project.
- Initialize Git and configure GitHub.
- Configure `.gitignore` and project documentation.
- Establish a feature-branch development workflow.

### Phase 1 — REST API & Swagger

- Configure ASP.NET Core controllers.
- Enable Swagger / OpenAPI documentation.
- Add an application health-check endpoint.

### Phase 2 — Database Integration

- Run Microsoft SQL Server 2022 in Docker.
- Configure Entity Framework Core 10.
- Implement the `TradingDbContext`.
- Create and apply Code First migrations.
- Configure the database connection using .NET User Secrets.
- Add a database connectivity endpoint.

### Phase 3 — Financial Instruments CRUD

- Create financial instruments.
- Retrieve all instruments or one instrument by ID.
- Update existing instruments.
- Delete instruments when database constraints permit.
- Validate instrument properties.
- Prevent duplicate instrument symbols.

### Phase 4 — Trading Orders Management

- Register BUY and SELL orders.
- Associate orders with existing financial instruments.
- Validate order types and quantities.
- Calculate order totals using the recorded instrument price.
- Store trading orders in SQL Server.
- Retrieve orders by ID or as a collection.
- Initialize orders with the `NEW` status.

### Phase 5 — Order Execution & Portfolio Positions

- Execute previously registered BUY and SELL orders.
- Transition successfully executed orders from `NEW` to `EXECUTED`.
- Reject execution attempts for already executed orders.
- Reject SELL execution when available holdings are insufficient.
- Create and update aggregated portfolio positions.
- Calculate weighted average acquisition prices after BUY executions.
- Reduce position quantities after SELL executions.
- Remove positions when their remaining quantity reaches zero.
- Use SQL Server transactions to update orders and positions together.
- Retrieve portfolio positions through REST endpoints.
- Calculate market value, cost basis, and unrealized profit or loss.

**Current limitations:** The MVP uses one aggregated simulated portfolio. Orders are executed using their recorded prices. Insufficient-holdings SELL orders remain `NEW` after a failed execution attempt. Concurrent-execution stress testing and automated tests have not yet been completed.

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

1. Create or retrieve a financial instrument.
2. Register a BUY or SELL trading order.
3. Verify that the order has the `NEW` status.
4. Execute the order through the execution endpoint.
5. Update the portfolio position and mark the order as `EXECUTED`.
6. Retrieve the updated portfolio positions.

### BUY Execution

A BUY increases the number of shares held.

If a position already exists, the weighted average acquisition price is recalculated:

**New Average Price = (Existing Quantity × Existing Average Price + Purchased Quantity × Purchase Price) / Total Quantity**

### SELL Execution

A SELL decreases the number of shares held.

The execution is refused if the requested quantity exceeds the available holdings.

When the remaining quantity reaches zero, the open portfolio position is removed.

### Order Status

| Status | Meaning |
|---|---|
| `NEW` | The order has been registered but not executed |
| `EXECUTED` | The order has been executed successfully |

Failed execution attempts do not automatically transition orders to a `REJECTED` status in the current MVP.

## Portfolio Valuation

Portfolio endpoints return position information together with calculated financial indicators.

| Indicator | Formula |
|---|---|
| Market Value | Quantity × Current Price |
| Cost Basis | Quantity × Average Acquisition Price |
| Unrealized P&L | Market Value − Cost Basis |

These calculations use simulated market prices and exclude fees, taxes, and realized P&L.

## Database

**Database Name:** `SazotechTradingDb`

### Database Tables

| Table | Description |
|---|---|
| `FinancialInstruments` | Stores financial instrument information |
| `TradingOrders` | Stores BUY and SELL orders |
| `PortfolioPositions` | Stores current aggregated holdings |
| `__EFMigrationsHistory` | Tracks EF Core migrations |

### Database Relationships

```text
FinancialInstruments
       |
       +------< TradingOrders
       |
       +------- PortfolioPositions
```

One financial instrument can have multiple trading orders and at most one aggregated portfolio position.

## Local Development Configuration

| Service | Host | Port |
|---|---|---|
| ASP.NET Core Web API | localhost | 5187 |
| Swagger UI | localhost | 5187 |
| SQL Server Docker (host) | 127.0.0.1 | 1435 |
| SQL Server (container) | Internal Docker network | 1433 |

**Important:** The SQL Server container uses port `1433` internally, while the Windows host connects using port `1435` to avoid conflicts with other SQL Server installations.

Database credentials must never be committed to Git.

## Running the Project

### 1. Start the SQL Server Container

```powershell
docker compose up -d
docker compose ps
```

### 2. Restore Dependencies

```powershell
dotnet restore
dotnet tool restore
```

### 3. Build the Application

```powershell
dotnet build
```

### 4. Configure Database Access

Configure the `ConnectionStrings:TradingDb` connection string using .NET User Secrets.

Keep passwords, connection credentials, and local `.env` files outside version control.

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

The order is initially saved with the `NEW` status.

### Execute the Order

**POST** `/api/orders/1/execute`

A successful execution returns HTTP `200 OK` and updates the corresponding portfolio position.

### Retrieve Portfolio Positions

**GET** `/api/positions`

Example response:

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

All values and identifiers above are illustrative.

## Manual Validation

The following scenarios have been exercised through Swagger and SQL Server during development:

- Financial instrument CRUD operations
- BUY order creation and execution
- SELL order creation and execution
- Insufficient-holdings SELL rejection
- Sequential double-execution prevention
- Weighted average acquisition price calculations
- Full position closure
- Portfolio retrieval and valuation

Automated unit and integration tests will be introduced in Phase 6.

## Git Workflow

Development uses dedicated feature branches, Conventional Commits, and GitHub Pull Requests.

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

## Next Phase — Unit Testing

Phase 6 will focus on introducing automated testing to protect core business rules.

Planned scenarios include:

- BUY execution and position creation
- Weighted average price calculation
- SELL execution and quantity reduction
- Insufficient-holdings rejection
- Sequential double-execution prevention
- Full position closure
- HTTP endpoint behavior and validation

## Future Improvements

- Automated integration testing against a test database
- Concurrency and transaction stress testing
- Additional order lifecycle states
- Realized profit and loss calculation
- Improved business validation and error handling
- Authentication and authorization
- Trading account and multi-portfolio support
- Centralized logging and observability
- Final containerized deployment

## Project Interface Preview

The images at the beginning of this README represent conceptual UI mockups.

The current implementation is a backend REST API with Swagger documentation. A custom trading dashboard has not yet been developed.

## Disclaimer

SAZOTECH-TRADING is an educational and portfolio project.

Financial instrument prices and trading operations are simulated. The application does not connect to live exchanges, provide investment advice, or execute real financial transactions.