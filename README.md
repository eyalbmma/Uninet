## Project Summary

Uninet is a financial automation and accounting document exchange platform that enables organizations to automatically transfer accounting documents between financial management systems in a fast, secure, and accurate way.

The platform reduces manual work, typing mistakes, reporting delays, phishing risks, fraud attempts, and regulatory exposure. Uninet allows organizations to:
- send and receive invoices, receipts, payment requests, and accounting documents electronically,
- synchronize accounting information between ERP and financial systems,
- automate document ingestion with a single click,
- improve operational efficiency and reduce cyber-security risks,
- minimize fake invoice exposure and simplify financial communication.

This repository contains the backend API, application services, data layer and batch services that implement Uninet core capabilities.

---

## What this codebase does (functional overview)

- Ingests accounting documents from external systems (push and pull).
- Stores raw documents in MongoDB and structured metadata in SQL Server (`BusinessData`).
- Runs batch processes that pull from external ERP APIs (ICount, GreenInvoice, Morning adapters).
- Maps and persists document metadata via stored procedures (e.g. `SP_InsertBusinessData`).
- Sends templated email notifications (OTP, verification, document links) via SMTP (SendGrid relay used in code).
- Provides an API surface for admin UI and external systems:
  - Admin flows: review, approve/reject, view document details.
  - External system flows: send documents (`/api/Documents/send_docs`).
- Handles authentication:
  - Admins: JWT signed with symmetric key (HS256).
  - External systems: JWT validated with RSA public key (RS256).
- Logs batch job activity to `Jobbatchlog`.

---

## Solution structure (actual projects & key folders)

- `UninetWebApi2` — ASP.NET Core Web API (entry point)
  - `Program.cs`, `Startup.cs`
  - `Controllers\` — `LoginController`, `RegisterController`, `DocumentsController`, `UninetInputController`, `UninetOutPutControllercs`, `GetStaticDataController`
  - `appsettings.json`, `appsettings.Development.json`, `appsettings.Production.json`
- `Uninet.APP` — Application layer services
  - `UninetInputAppService`, `UninetOutPutAppService`, `UserServiceApp`, `TokenService`
- `Uninet.DATA` — Data access and batch services
  - `Services\` — `UninetInputDataAccess`, `UninetOutputDataAccess`, `uninetBatchDataAccess`, `BatchDataMailassist`, `DataMailassist`
  - `Services\MultipleContext\` — EF Core contexts: `UninetContext`, `UninetBatchContext`
- `Uninet.Domain` — Entities, DTOs, interfaces, repository implementations
  - `Domain\Classes` — `Repository<TDbContext>`, `BatchRepository<TDbContext>`
  - `Domain\Interfaces` — `IRepository<TDbContext>`, `IBatchRepository<TDbContext>`

---

## Key classes and responsibilities

- `UninetWebApi2\Startup.cs`
  - DI, authentication schemes, authorization policies, Swagger, middleware registration.
- Controllers
  - `LoginController` — login, refresh token, Google sign-in, OTP flows.
  - `DocumentsController` — external push API (`send_docs`) protected by `ExternalPolicy`.
  - `UninetOutPutControllercs` — admin document list/approve endpoints protected by `AdminPolicy`.
  - `UninetInputController`, `GetStaticDataController`, `RegisterController` — related admin/input endpoints.
- Application services (`Uninet.APP`)
  - Provide thin facades over `Uninet.DATA` to controllers.
- Data access (`Uninet.DATA`)
  - `Repository<TDbContext>` / `BatchRepository<TDbContext>` — generic EF & stored procedure helpers (`ExecuteGetSP`, `ExecuteGetSPAsync`).
  - `UninetInputDataAccess` / `UninetOutputDataAccess` / `uninetBatchDataAccess` — integration, ETL, mapping, MongoDB operations.
  - `BatchDataMailassist` — email templating (fetch templates via `SP_GetHtmlBody`) and SMTP sending.
- Domain entities
  - `AdminUsers`, `BusinessData`, `Jobbatchlog`, `OTPHtmlBody`, `SystemsEndpoints`, etc.

---

## Technical architecture

- Architecture style: Layered (API -> App services -> Data layer -> DB)
- Data stores:
  - SQL Server via EF Core (`UninetContext`, `UninetBatchContext`)
  - MongoDB for raw document collections and external system caches
- Integration patterns:
  - HTTP client calls to external ERPs (ICount, Morning, GreenInvoice)
  - Stored procedures for templating and bulk inserts (e.g., `SP_GetHtmlBody`, `SP_InsertGreenvoiceJsonDetailsIntoDB`, `SP_InsertBusinessData`, `SP_UpdateBusinessDataEmailSent`)
  - SMTP for email notifications (`SmtpClient` with `smtp.sendgrid.net` in code)
- Authentication:
  - Two JWT schemes in `Startup.cs`: `AdminScheme` (HS256) and `ExternalScheme` (RS256). Authorization policies `AdminPolicy` and `ExternalPolicy`.
- Dependency injection:
  - Services registered in `Startup.ConfigureServices` with appropriate lifetimes (`AddScoped`, `AddSingleton`).
  - `IMongoClient` registered as singleton.
- Background/batch processing:
  - Batch classes (e.g., `uninetBatchDataAccess`) are used by scheduled jobs or manual invocations to pull and process external documents.
- Observability:
  - Basic use of `ILogger<T>` in some components and `appsettings.json` logging configuration.
  - Job activity logged to `Jobbatchlog` table for auditing.
- Security considerations:
  - OTP and email GUID verification implemented.
  - Secrets are read from `appsettings.*` (placeholders present). Move to secure store for production.

---

## Technologies used (as found in repository)

- .NET 6, C# 10
- ASP.NET Core Web API
- Entity Framework Core (SQL Server provider)
- MongoDB (official driver)
- SMTP / SendGrid relay (`SmtpClient` in code)
- JWT (HS256) and RSA-signed JWT validation (RS256) for external systems
- Twilio referenced (SMS/voice integrations)
- Stored procedures via `FromSqlRaw`/`ExecuteSqlRawAsync`
- Swagger/OpenAPI (`AddSwaggerGen` is used)

Note: Docker, Redis, message brokers (RabbitMQ/Azure SB), CI/CD pipelines, or structured telemetry (e.g., Application Insights) were not found in the scanned code — these are not assumed to exist.

---

## System flow (end-to-end)

1. External system pushes documents or batch job pulls documents from external ERP.
2. Raw documents stored to MongoDB (collections like `UninetGreenVoiceCollection`, `Icount`, `IcountDocInfo`).
3. ETL batch service (`uninetBatchDataAccess` / `UninetInputDataAccess`) maps documents and calls stored procedures to insert metadata into SQL (`BusinessData` via `SP_InsertBusinessData`).
4. When document requires action, `BatchDataMailassist` fetches template via `SP_GetHtmlBody`, replaces placeholders with `ReplaceDynamicPlaceholders` and sends email via SMTP.
5. Admin UI (consuming endpoints in `UninetWebApi2`) lists documents, shows details and approves/rejects documents. Approvals trigger further pushes to target ERP endpoints (via adapters in `UninetOutputDataAccess`).
6. All batch operations are logged to `Jobbatchlog`.

---

## API surface (high level)

- Authentication endpoints: `/api/Login/*` (`LoginWithEmailPassword`, `RefreshToken`, `GoogleSignIn`, `ForgotPassword`, `ResetPassword`)
- External ingest: `POST /api/Documents/send_docs` (protected by `ExternalPolicy`)
- Admin operations:
  - `GET /api/UninetOutPutControllercs/GetDigitalDocumentToApproveListByUser` (AdminPolicy)
  - `POST /api/UninetOutPutControllercs/ShowDigitalDocumentDetails` (AdminPolicy)
- Static content: `GET /api/GetStaticData/{questionNumber}/{language}` (serves content from Mongo)
- Swagger UI is enabled in `Startup` (visit `/swagger` when running).

For full endpoint details, run the API locally and inspect the Swagger UI.

---

## Developer setup (quick start)

Prerequisites
- .NET 6 SDK
- SQL Server instance accessible by `ConnectionStrings:AppConnectionString`
- MongoDB instance (connection in `ConnectionStrings:MongoDb`)
- SMTP credentials (SendGrid or other SMTP relay)
- RSA public key for external JWT validation (production path is currently hard-coded in `Startup.cs` — update to a configurable path)

Configuration
1. Copy and edit `UninetWebApi2\appsettings.Development.json`:
   - Set `ConnectionStrings:AppConnectionString`
   - Set `ConnectionStrings:MongoDb`
   - Set `jwtTokenConfig:secret` to a Base64 secret (local only)
   - Set `EmailSettings:Username` and `EmailSettings:Password`
   - Set `UrlRedirect:*` entries
   - Set `EncryptedUserId:key` and `EncryptedUserId:iv`
2. Update RSA public key loading in `Startup.cs` or provide path accessible to the runtime.

Run locally
- From solution folder run:
  - __dotnet build__
  - __dotnet run --project UninetWebApi2__
- Or open solution in Visual Studio 2022, set `UninetWebApi2` as startup project and run (F5).

Database
- Provision SQL Server and apply schema/stored procedures. Stored-procedure SQL scripts are not present in the repository scan — confirm with DB admins or repository owners.
- MongoDB collections will be created automatically on first use.

Secrets
- Use environment variables or a secure secret store for production credentials (do not commit secrets to source).

---

## Important configuration keys (found in `appsettings.*`)

- `jwtTokenConfig` — JWT secret, encryption key, expirations
- `ConnectionStrings:AppConnectionString` — SQL Server
- `ConnectionStrings:MongoDb` — MongoDB
- `EmailSettings` — SMTP username and password
- `UrlRedirect:Console`, `UrlRedirect:SignUp`, `UrlRedirect:Homepage` — redirect URLs used in email templates
- `EncryptedUserId:key` and `EncryptedUserId:iv` — used to encrypt user id in responses

---
