# MiniSaaS

A production-oriented **multi-tenant SaaS backend** built with **ASP.NET Core 10**, **Entity Framework Core 10**, **SQL Server**, **JWT Authentication**, **Role-Based Authorization**, **Hangfire**, **FluentValidation**, **xUnit**, and **Moq**.

The project is designed as a clean, scalable, and extensible foundation for building multi-tenant SaaS applications.

---

## 🚀 Features

* Multi-Tenant Architecture
* Tenant Isolation
* JWT Authentication
* Role-Based Authorization
* User Management
* Tenant Management
* Password Hashing
* FluentValidation
* Global Exception Handling
* Correlation ID Middleware
* Soft Delete
* Pagination
* Result / Response Pattern
* Entity Framework Core
* SQL Server
* Hangfire Background Jobs
* Recurring Jobs
* Background Job Logging
* xUnit Unit Testing
* Moq Mocking
* Swagger / OpenAPI
* Repository Pattern
* Unit of Work
* Dependency Injection
* EF Core Global Query Filters

---

# 🏗️ Architecture

The project follows a layered architecture inspired by **Clean Architecture principles**.

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
├── tests
│   │
│   ├── MiniSaaS.Application.Tests
│   └── MiniSaaS.API.Tests
│
├── docs
│   └── MiniSaaS.postman_collection.json
│
├── README.md
└── MiniSaaS.sln
```

### Layer Responsibilities

| Layer              | Responsibility                                                        |
| ------------------ | --------------------------------------------------------------------- |
| **API**            | HTTP endpoints, middleware, authentication and exception handling     |
| **Application**    | Business logic, DTOs, validators and service abstractions             |
| **Domain**         | Core entities, enums and domain abstractions                          |
| **Infrastructure** | EF Core, SQL Server, repositories, authentication and background jobs |
| **Tests**          | Unit and controller tests                                             |

---

# 🛠️ Technology Stack

| Technology                 | Purpose                  |
| -------------------------- | ------------------------ |
| ASP.NET Core 10            | Web API                  |
| Entity Framework Core 10   | ORM / Data Access        |
| SQL Server                 | Database                 |
| JWT                        | Authentication           |
| ASP.NET Core Authorization | Role-based authorization |
| Hangfire                   | Background jobs          |
| FluentValidation           | Request validation       |
| Swagger / OpenAPI          | API documentation        |
| xUnit                      | Unit testing             |
| Moq                        | Mocking                  |
| C#                         | Programming language     |

---

# 🌐 Multi-Tenancy

The API uses a tenant-based architecture.

Tenant-aware endpoints require:

```http
X-Tenant-Id: 1
```

Example:

```http
GET /api/users?pageNumber=1&pageSize=10
X-Tenant-Id: 1
Authorization: Bearer <token>
```

## Tenant Request Flow

```text
Client
   │
   │ X-Tenant-Id
   ▼
TenantMiddleware
   │
   ├── Validate Header
   ├── Validate Tenant ID
   ├── Check Tenant Exists
   └── Check Tenant Is Active
   │
   ▼
TenantContext
   │
   ▼
Application Service
   │
   ▼
EF Core Global Query Filter
   │
   ▼
Tenant-specific Data
```

The tenant middleware:

1. Checks whether the endpoint requires a tenant.
2. Reads the `X-Tenant-Id` header.
3. Validates that the tenant ID is a positive integer.
4. Checks that the tenant exists.
5. Checks that the tenant is active.
6. Stores the tenant ID in the tenant context.
7. Allows the request to continue.

Invalid tenant requests are rejected before reaching the controller.

---

# 🔐 Tenant Isolation

Tenant isolation is implemented at the persistence level.

User queries are automatically filtered according to the current tenant.

Conceptually:

```text
Request
   │
   ▼
X-Tenant-Id
   │
   ▼
TenantMiddleware
   │
   ▼
TenantContext
   │
   ▼
EF Core Global Query Filter
   │
   ▼
Tenant-specific Records
```

This prevents users belonging to one tenant from accessing users belonging to another tenant.

Database-level constraints should also be used where appropriate to provide an additional layer of protection.

---

# 🔑 Authentication

The application uses **JWT Bearer Authentication**.

## Authentication Flow

```text
Client
   │
   │ Email + Password
   ▼
AuthController
   │
   ▼
AuthService
   │
   ├── Validate Tenant
   ├── Find User
   ├── Verify Password
   └── Generate JWT
   │
   ▼
Client
   │
   └── JWT Access Token
```

The JWT contains information such as:

* User ID
* Tenant ID
* Email
* Role

Example response:

```json
{
  "success": true,
  "data": {
    "accessToken": "eyJhbGciOiJIUzI1NiIs...",
    "expiresAt": "2026-08-16T22:00:00Z"
  },
  "message": "Login successful."
}
```

---

# 👥 Roles

The application currently supports:

* `Admin`
* `Member`

Roles are included in the JWT token and can be used for authorization.

Example:

```csharp
[Authorize(Roles = "Admin")]
```

This restricts the endpoint to administrators.

---

# 🔒 Password Security

Passwords are never stored as plain text.

When creating a user:

```text
Password
   │
   ▼
IPasswordHasher
   │
   ▼
Password Hash
   │
   ▼
Database
```

During authentication:

```text
Password
   │
   ▼
IPasswordHasher.Verify()
   │
   ▼
Stored Password Hash
   │
   ▼
Authentication Result
```

---

# 🧪 Validation

The project uses **FluentValidation** for request validation.

Validation is performed before executing business logic.

```text
HTTP Request
     │
     ▼
Validation
     │
 ┌───┴────┐
 │        │
Valid   Invalid
 │        │
 ▼        ▼
Service  Error Response
```

Example validation errors:

```json
{
  "success": false,
  "data": null,
  "message": "One or more validation errors occurred.",
  "errorCode": 1000,
  "errors": [
    "Full name is required.",
    "Email is required."
  ]
}
```

---

# ⚠️ Error Handling

The API follows a consistent response structure.

## Successful Response

```json
{
  "success": true,
  "data": {},
  "message": "Operation completed successfully.",
  "errorCode": null,
  "errors": null
}
```

## Error Response

```json
{
  "success": false,
  "data": null,
  "message": "User not found.",
  "errorCode": 1001,
  "errors": null
}
```

The application provides centralized exception handling for unexpected errors and business exceptions.

---

# 🗑️ Soft Delete

Users are not physically deleted from the database.

Instead, the user is deactivated:

```text
Delete User
    │
    ▼
IsActive = false
    │
    ▼
Record remains in Database
```

This preserves historical data while preventing inactive users from accessing the system.

---

# 📄 Pagination

User endpoints support pagination.

Example:

```http
GET /api/users?pageNumber=1&pageSize=10
```

Pagination helps prevent large datasets from being loaded into memory in a single request.

---

# 🗄️ Database

The project uses **Microsoft SQL Server** with **Entity Framework Core**.

EF Core manages:

* Database schema
* Migrations
* Relationships
* Global query filters
* Persistence
* Database queries

---

# 🔄 Migrations

Create a migration:

```bash
dotnet ef migrations add InitialCreate
```

Apply migrations:

```bash
dotnet ef database update
```

The application also applies pending migrations during startup.

---

# 🌱 Database Seeding

The application includes automatic database seeding.

A demo tenant and administrator account are created when the required data does not exist.

### Demo Tenant

```text
Name: Demo Tenant
Slug: demo-tenant
ID: 1
```

### Demo Administrator

```text
Full Name: System Admin
Email: admin@minisaas.com
Password: Admin@123456
Role: Admin
Tenant ID: 1
```

> ⚠️ These credentials are intended for local development only. Replace them with secure credentials in production.

---

# ⚙️ Hangfire

The application uses **Hangfire** for background processing.

The project includes a recurring job that calculates the number of active users for every tenant.

Example:

```text
Tenant 1 → 4 active users
Tenant 2 → 10 active users
Tenant 3 → 0 active users
```

The job runs independently from HTTP requests.

Hangfire is useful for operations that are:

* Scheduled
* Periodic
* Long-running
* Not required to block an HTTP request

---

# 📊 Active Users Background Job

The background job:

1. Loads tenants.
2. Calculates active users.
3. Logs the results.

Conceptually:

```sql
SELECT TenantId, COUNT(*)
FROM Users
WHERE IsActive = 1
GROUP BY TenantId
```

Example logs:

```text
Active users job started.

Tenant 1 has 4 active users.
Tenant 5 has 0 active users.

Active users job completed.
```

---

# 📋 Hangfire Dashboard

Hangfire Dashboard provides visibility into:

* Enqueued jobs
* Processing jobs
* Scheduled jobs
* Recurring jobs
* Failed jobs
* Succeeded jobs
* Servers
* Retries

During development, the dashboard can be accessed through:

```text
https://localhost:7093/hangfire
```

or:

```text
http://localhost:5023/hangfire
```

depending on the selected application URL.

> ⚠️ The Hangfire Dashboard should be protected with proper authorization in production.

---

# 🔌 API Documentation

## Swagger

When the application is running:

```text
https://localhost:7093/swagger
```

Swagger can be used to:

* Explore endpoints
* View request models
* View response models
* Test API endpoints
* Authorize using JWT Bearer tokens

---

# 📮 Postman

A Postman collection is included in the repository:

```text
docs/postman/MiniSaaS.postman_collection.json
```

Import the collection into Postman and configure:

```text
baseUrl
tenantId
token
```

The collection contains examples for:

```text
Tenants
├── Add Tenant
├── Get Tenant
├── Test Not Found
├── Test Conflict
└── Test Validation

Users
├── Add User
├── Get Users
├── Update User
├── Delete User
├── Validation Test
└── Conflict Test

Auth
└── Login
```

---

# 🏢 Tenant API

## Create Tenant

```http
POST /api/tenants
```

Request:

```json
{
  "name": "Globex Corporation",
  "slug": "globex"
}
```

Response:

```json
{
  "success": true,
  "data": {
    "id": 1,
    "name": "Globex Corporation",
    "slug": "globex"
  },
  "message": "Tenant created successfully."
}
```

---

## Get Tenant

```http
GET /api/tenants/{id}
```

Example:

```http
GET /api/tenants/1
```

---

# 🔐 Authentication API

## Login

```http
POST /api/auth/login
```

Required header:

```http
X-Tenant-Id: 1
```

Request:

```json
{
  "email": "admin@minisaas.com",
  "password": "Admin@123456"
}
```

Successful response:

```json
{
  "success": true,
  "data": {
    "accessToken": "JWT_TOKEN",
    "expiresAt": "2026-08-16T22:00:00Z"
  },
  "message": "Login successful."
}
```

---

# 👤 Users API

All user endpoints require:

```http
X-Tenant-Id: <tenantId>
Authorization: Bearer <token>
```

## Get Users

```http
GET /api/users?pageNumber=1&pageSize=10
```

Example:

```http
GET /api/users?pageNumber=1&pageSize=10
X-Tenant-Id: 1
Authorization: Bearer <token>
```

---

## Create User

```http
POST /api/users
```

Headers:

```http
X-Tenant-Id: 1
Authorization: Bearer <token>
Content-Type: application/json
```

Request:

```json
{
  "fullName": "Omar Ali",
  "email": "omar@example.com",
  "role": 1,
  "password": "Password@123"
}
```

The password is hashed before being stored in the database.

---

## Update User

```http
PUT /api/users/{id}
```

Example:

```http
PUT /api/users/1
```

Request:

```json
{
  "fullName": "Omar Ali Updated",
  "email": "omar.updated@example.com",
  "role": 2
}
```

---

## Delete User

```http
DELETE /api/users/{id}
```

The operation is implemented as a soft delete.

The record remains in the database while:

```text
IsActive = false
```

---

# 🔄 Request Flow

A typical authenticated request follows this flow:

```text
Client
  │
  │ X-Tenant-Id
  │ Authorization: Bearer JWT
  ▼
ASP.NET Core Middleware
  │
  ├── Authentication
  ├── Authorization
  └── Tenant Middleware
  │
  ▼
Controller
  │
  ▼
Application Service
  │
  ▼
Unit of Work / Repository
  │
  ▼
Entity Framework Core
  │
  ▼
SQL Server
```

---

# 🔐 Authentication Flow

```text
Client
  │
  │ Email + Password
  ▼
AuthController
  │
  ▼
AuthService
  │
  ├── Validate Tenant
  ├── Find User
  ├── Verify Password
  └── Generate JWT
  │
  ▼
Client
  │
  └── JWT Access Token
```

---

# 🏢 Multi-Tenant Request Flow

```text
Client
  │
  │ X-Tenant-Id: 1
  ▼
TenantMiddleware
  │
  ├── Validate Header
  ├── Validate Tenant ID
  ├── Check Tenant Exists
  └── Check Tenant Is Active
  │
  ▼
TenantContext
  │
  ▼
Application Service
  │
  ▼
EF Core Global Query Filter
  │
  ▼
Tenant-specific Records
```

---

# ⚡ Background Job Flow

```text
Hangfire Scheduler
       │
       ▼
ActiveUsersJob
       │
       ▼
Load Tenants
       │
       ▼
Count Active Users
       │
       ▼
Log Results
```

---

# 🧪 Testing

The project uses:

* **xUnit**
* **Moq**

Unit tests cover important business scenarios.

### Tenant Service

* Create tenant successfully
* Create tenant with invalid data
* Create tenant with duplicate slug
* Get existing tenant
* Get non-existing tenant

### User Service

* Get users
* Create user successfully
* Create user without tenant context
* Create user with duplicate email
* Password hashing
* Update user
* Update non-existing user
* Update with duplicate email
* Delete user
* Delete non-existing user

### Auth Service

* Login successfully
* Login without tenant context
* User does not exist
* Invalid password
* JWT token generation

### Controllers

Controller tests verify:

* HTTP status codes
* Service invocation
* Successful responses
* Error responses
* Created responses
* Not Found responses

---

# 🧩 Unit Test Structure

Moq is used to isolate dependencies from the system under test.

Example:

```csharp
var unitOfWork = new Mock<IUnitOfWork>();

var tenantRepository =
    new Mock<IRepository<Tenant>>();

unitOfWork
    .Setup(x => x.Repository<Tenant>())
    .Returns(tenantRepository.Object);
```

The goal is to test application logic without requiring a real database.

---

# 🚀 Running the Project

## Prerequisites

Install:

* .NET 10 SDK
* SQL Server
* Visual Studio 2022+ or Visual Studio Code
* Postman (optional)

---

## Clone the Repository

```bash
git clone <YOUR_GITHUB_REPOSITORY_URL>
```

Navigate to the project:

```bash
cd MiniSaaS
```

---

# 🗄️ Configure Database

Update the connection string in:

```text
appsettings.json
```

Example:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MiniSaaSDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Use your own SQL Server configuration.

---

# 🔑 Configure JWT

Example:

```json
{
  "Jwt": {
    "Issuer": "MiniSaaS",
    "Audience": "MiniSaaS.Client",
    "SecretKey": "YOUR_SECRET_KEY"
  }
}
```

For production:

* Do not commit real secrets.
* Use environment variables.
* Use .NET User Secrets for local development.
* Use a secure secret manager in production.

---

# ▶️ Run the Application

From the API project:

```bash
dotnet run
```

The application will start on URLs similar to:

```text
https://localhost:7093
http://localhost:5023
```

---

# 📚 Swagger

Open:

```text
https://localhost:7093/swagger
```

or:

```text
http://localhost:5023/swagger
```

To test protected endpoints:

1. Call `POST /api/auth/login`.
2. Include `X-Tenant-Id: 1`.
3. Copy the returned JWT.
4. Click **Authorize** in Swagger.
5. Enter:

```text
Bearer YOUR_TOKEN
```

6. Call the protected endpoints.

---

# 🔒 Security Considerations

For production environments:

* Replace demo credentials.
* Use a strong JWT secret.
* Store secrets outside source control.
* Use HTTPS.
* Add proper authorization policies.
* Add rate limiting.
* Consider refresh tokens.
* Add audit logging.
* Add database-level unique constraints.
* Protect the Hangfire Dashboard.
* Avoid logging sensitive information.
* Validate all incoming requests.
* Consider tenant authorization based on the authenticated user's identity rather than trusting only a client-supplied tenant header.

---

# ⚙️ Concurrency Considerations

The application considers concurrency in several areas.

### Database

Entity Framework Core operations use asynchronous APIs:

```csharp
await _unitOfWork.SaveChangesAsync(cancellationToken);
```

### Hangfire

Hangfire manages background job execution and server coordination.

Multiple workers can process jobs concurrently.

### Tenant Context

Tenant context is scoped to the current request and is not shared between requests.

### Duplicate Data

The application checks for existing tenant slugs and user emails before inserting records.

Database-level unique constraints should also be used to protect against race conditions where multiple requests execute simultaneously.

---

# 🧱 Project Principles

The project follows several backend development principles.

### Separation of Concerns

Each layer has a clearly defined responsibility.

### Dependency Inversion

Application services depend on abstractions rather than infrastructure implementations.

### Async Programming

Database and background operations use asynchronous APIs where appropriate.

### DTOs

Entities are not directly exposed through API contracts.

### Result Pattern

Application operations return a consistent:

```text
ResultDto<T>
```

structure.

### Tenant Isolation

Tenant-aware data is automatically scoped to the current tenant.

### Secure Authentication

Passwords are hashed and JWT tokens are used for authentication.

### Soft Delete

Users are deactivated instead of physically removed.

---

# 🔮 Future Improvements

Possible future enhancements include:

* Refresh Tokens
* Email Verification
* Password Reset
* Admin-only Tenant Management
* Tenant Registration Flow
* Subscription Management
* Billing Integration
* Audit Logs
* Rate Limiting
* API Versioning
* Redis Caching
* Distributed Locking
* Health Checks
* Centralized Logging
* OpenTelemetry
* Docker Support
* CI/CD Pipeline
* Integration Tests
* Testcontainers
* Role / Permission Management
* Tenant-based Configuration
* Tenant Usage Limits

---

# 📁 Repository Structure

```text
MiniSaaS/
│
├── src/
│   │
│   ├── MiniSaaS.Domain/
│   │
│   ├── MiniSaaS.Application/
│   │
│   ├── MiniSaaS.Infrastructure/
│   │
│   └── MiniSaaS.API/
│
├── tests/
│   │
│   ├── MiniSaaS.Application.Tests/
│   │
│   └── MiniSaaS.API.Tests/
│
├── docs/
│   └── MiniSaaS.postman_collection.json
│
├── README.md
└── MiniSaaS.sln
```

---

# 🔑 Example Credentials

For local development only:

### Tenant

```text
ID: 1
Name: Demo Tenant
Slug: demo-tenant
```

### Admin

```text
Email: admin@minisaas.com
Password: Admin@123456
Role: Admin
```

Required tenant header:

```http
X-Tenant-Id: 1
```

> ⚠️ Never use these credentials in a production environment.

---

# 📄 License

This project is intended for **learning, demonstration, and portfolio purposes**.

---

# 👨‍💻 Author

Built as a backend SaaS architecture project using modern **ASP.NET Core** technologies, focusing on:

* Clean Architecture
* Multi-Tenancy
* Secure Authentication
* Scalable Backend Design
* Background Processing
* Validation
* Testing
* Maintainable Code

---
