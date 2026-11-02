# SAZOTECH-TRADING — Local Port Reference

## Purpose

This document describes the local network ports used by SAZOTECH-TRADING and lists ports reserved for other development projects.

Its purpose is to prevent port conflicts when running multiple applications, databases, and Docker containers on the same Windows workstation.

## SAZOTECH-TRADING Ports

| Component | Host | Host Port | Container Port | Protocol |
|---|---|---:|---:|---|
| ASP.NET Core Web API | localhost | 5187 | N/A | HTTP |
| Swagger UI | localhost | 5187 | N/A | HTTP |
| SQL Server 2022 Docker | 127.0.0.1 | 1435 | 1433 | TCP |

### SQL Server Port Mapping

```text
Windows Host
127.0.0.1:1435
       |
       | Docker port forwarding
       v
SQL Server Container
Port 1433
       |
       v
SazotechTradingDb
```

**Important:** SAZOTECH-TRADING uses host port `1435` for SQL Server. Port `1433` is used inside its Docker container.

Do not change the host port to `1433` without checking for conflicts with existing SQL Server installations.

## Reserved Local Development Ports

The following ports have been identified as in use or reserved for other development environments. Their current availability must be checked before starting new services.

| Port | Reservation |
|---:|---|
| 1433 | Existing SQL Server installation |
| 1435 | SAZOTECH-TRADING SQL Server Docker |
| 3000 | Other development services |
| 3001 | Other development services |
| 4040 | Other development services |
| 5050 | Other development services |
| 5187 | SAZOTECH-TRADING ASP.NET Core API |
| 5432 | Other database services |
| 5433 | Other database services |
| 5434 | Other database services |
| 7077 | Other development services |
| 8080 | Other development services |
| 8081 | Other development services |
| 8083 | Other development services |
| 8084 | Other development services |
| 8088 | Other development services |
| 8501 | Other development services |
| 9000 | Other development services |
| 9092 | Other development services |
| 9864 | Other development services |
| 9870 | Other development services |

This list records local reservations, not guaranteed active listeners.

## Checking Port Usage on Windows

To check a specific port in PowerShell:

```powershell
Get-NetTCPConnection -LocalPort 1435 -ErrorAction SilentlyContinue
```

To inspect listening TCP ports:

```powershell
Get-NetTCPConnection -State Listen |
    Sort-Object LocalPort |
    Select-Object LocalAddress, LocalPort, OwningProcess
```

## Checking Docker Port Mappings

Run from the repository root:

```powershell
docker compose ps
```

Expected SQL Server mapping:

```text
127.0.0.1:1435->1433/tcp
```

## Connection Settings

For SQL Server Management Studio (SSMS):

- Server: `127.0.0.1,1435`
- Authentication: SQL Server Authentication
- Database: `SazotechTradingDb`
- Credentials: configured locally; never committed to Git

For the ASP.NET Core API:

- API base URL: `http://localhost:5187`
- Swagger UI: `http://localhost:5187/swagger`

## Security Notes

- Keep SQL Server credentials in local environment configuration and .NET User Secrets.
- Never commit the actual `.env` file.
- Use `.env.example` only for placeholder values.
- Do not publish database ports to public network interfaces unnecessarily.
- Preserve Docker database volumes when stopping or recreating containers.

## Maintenance

Update this reference whenever a service port or Docker mapping changes.

Verify actual port availability before assigning a port to a new development project.