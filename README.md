# MiniSaaS

A production-oriented multi-tenant SaaS backend built with **ASP.NET Core Web API**, **Entity Framework Core**, **SQL Server**, **JWT Authentication**, **Role-Based Authorization**, **Hangfire**, **FluentValidation**, **xUnit**, and **Moq**.

The project is designed as a clean and extensible foundation for building multi-tenant SaaS applications.

---

## 🚀 Features

- Multi-Tenant Architecture
- Tenant Isolation
- JWT Authentication
- Role-Based Authorization
- User Management
- Tenant Management
- Password Hashing
- FluentValidation
- Global Exception Handling
- Correlation ID Middleware
- Soft Delete for Users
- Pagination
- Result/Response Pattern
- Entity Framework Core
- SQL Server
- Hangfire Background Jobs
- Recurring Jobs
- Background Job Logging
- xUnit Unit Testing
- Moq Mocking

---

## 🏗️ Architecture

The project follows a layered architecture inspired by Clean Architecture principles.

```text
MiniSaaS
│
├── src
│   │
│   ├── MiniSaaS.API
│   │   ├── Controllers
│   │   ├── Middleware
│   │   ├── ExceptionHandling
│   │   ├── Extensions
│   │   └── Program.cs
│   │
│   ├── MiniSaaS.Application
│   │   ├── Auth
│   │   ├── Users
│   │   ├── Tenants
│   │   ├── Common
│   │   └── DependencyInjection.cs
│   │
│   ├── MiniSaaS.Domain
│   │   ├── Entities
│   │   ├── Enums
│   │   └── Common
│   │
│   └── MiniSaaS.Infrastructure
│       ├── Persistence
│       ├── MultiTenancy
│       ├── Authentication
│       ├── Identity
│       ├── BackgroundJobs
│       ├── Repositories
│       ├── UnitOfWork
│       └── DependencyInjection.cs
│
└── tests
    │
    └── MiniSaaS.Tests
        ├── API
        └── Services

## API Documentation

### Swagger

When the application is running:

https://localhost:7093/swagger

### Postman

The Postman collection is available at:

`docs/postman/MiniSaaS.postman_collection.json`

Import the collection into Postman and configure:

- `baseUrl`
- `tenantId`
- `token`
