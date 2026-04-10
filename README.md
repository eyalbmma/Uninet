 Uninet Backend API

A production-style backend system built with .NET 6, designed to manage users, business entities, financial documents, and integrations with external accounting systems.

---

## 🚀 Overview

Uninet is a layered backend system that handles:

- User authentication (Email / OTP / Refresh Tokens)
- Business onboarding and management
- Financial document workflows (approval / rejection)
- Billing operations
- Webhook ingestion from external systems
- Integration with accounting platforms (e.g. Green Invoice / iCount)

The system supports both **internal users** and **external partner systems**, each with a different authentication model.

---

## 🏗️ Architecture

The project follows a clean **Layered Architecture**:


Controllers (API Layer)
↓
Application Services (Business Logic)
↓
Data Access Layer (Repositories / Services)
↓
Databases (SQL Server + MongoDB)


### Layers

- **UninetWebApi2** – Main API (entry point)
- **Uninet.APP** – Business logic layer
- **Uninet.DATA** – Data access layer
- **Uninet.Domain** – Entities, DTOs, contracts

---

## ⚙️ Tech Stack

- .NET 6 (ASP.NET Core Web API)
- Entity Framework Core
- SQL Server
- MongoDB
- JWT Authentication
- Swagger / OpenAPI
- GitHub Actions (CI/CD)
- Azure Web App

---

## 🔐 Authentication Model

### Internal Users
- JWT authentication
- Symmetric key signing
- Role-based authorization (`AdminPolicy`)

### External Systems
- OAuth-style `client_credentials` flow
- RSA-signed JWT tokens
- IP whitelisting for secure access

---

## 📦 Core Features

- User registration (OTP + Email verification)
- Login / Logout / Refresh tokens
- Business onboarding (`Join_entity`)
- Billing management
- Business partners management
- Document ingestion and approval workflows
- Webhook endpoints for external integrations
- Financial data queries

---

## 🔗 External Integrations

- Accounting systems (Green Invoice / iCount)
- Email services (SMTP / API)
- MongoDB document storage
- External REST APIs

---

## ▶️ Running the Project

### Prerequisites

- .NET 6 SDK
- SQL Server
- MongoDB

### Run

```bash
dotnet restore
dotnet build
dotnet run --project UninetWebApi2

Swagger:

https://localhost:7202/swagger
⚠️ Configuration

All sensitive values were removed from this repository.

To run locally, create your own configuration:

Database connection string
MongoDB URI
JWT secrets
Email API key
Google OAuth credentials
🧠 Key Highlights
Clean separation of concerns (SOC + SRP)
Hybrid data architecture (SQL + MongoDB)
Dual authentication model (users + external systems)
Secure external integrations
Real-world backend architecture
💬 Interview Summary

This project demonstrates:

Backend architecture design in .NET
Authentication and security patterns (JWT + RSA)
Integration with external systems
Multi-database usage
Real-world API design and workflows
👤 Author

Eyal Berda