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
├── tests
│   │
│   ├── MiniSaaS.Tests
│       ├── API
│       └── Services
│
├── docs
│   │
│   └── MiniSaaS.postman_collection.json
│
├── README.md
└── MiniSaaS.sln


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

Technology Stack
Technology	Purpose
ASP.NET Core 10	Web API
Entity Framework Core 10	ORM
SQL Server	Database
JWT	Authentication
ASP.NET Core Authorization	Role-based authorization
Hangfire	Background jobs
FluentValidation	Request validation
Swagger / OpenAPI	API documentation
xUnit	Unit testing
Moq	Mocking dependencies
C#	Programming language
Multi-Tenancy

The API uses a tenant-based architecture.

Tenant-aware endpoints require the following HTTP header:

X-Tenant-Id: 1

Example:

GET /api/users
X-Tenant-Id: 1
Authorization: Bearer <token>

The tenant middleware:

Checks whether the endpoint requires a tenant.
Reads the X-Tenant-Id header.
Validates that the tenant ID is a positive integer.
Checks that the tenant exists and is active.
Stores the tenant ID in the tenant context.
Allows the request to continue.

Invalid tenant requests are rejected before reaching the controller.

Tenant Isolation

Tenant isolation is implemented at the persistence level.

User queries are automatically filtered by the current tenant.

Conceptually:

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
Tenant-specific data

This prevents users from one tenant from accessing users belonging to another tenant.

Authentication

The project uses JWT Bearer Authentication.

Authentication flow:

Login
  │
  ▼
Validate Tenant
  │
  ▼
Find User
  │
  ▼
Verify Password
  │
  ▼
Generate JWT
  │
  ▼
Return Access Token

JWT contains information such as:

User ID
Tenant ID
Email
Role

Example:

{
  "accessToken": "eyJhbGciOiJIUzI1NiIs...",
  "expiresAt": "2026-08-16T22:00:00Z"
}
Roles

The application currently supports:

Admin
Member

Roles are included in the JWT token and can be used for authorization.

Example:

[Authorize(Roles = "Admin")]

This allows an endpoint to be restricted to administrators.

Password Security

Passwords are never stored as plain text.

When creating a user:

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

During login:

Password
   │
   ▼
IPasswordHasher.Verify()
   │
   ▼
Password Hash
   │
   ▼
Authentication Result
API Endpoints
Tenants
Create Tenant
POST /api/tenants

Request:

{
  "name": "Globex Corporation",
  "slug": "globex"
}

Response:

{
  "success": true,
  "data": {
    "id": 1,
    "name": "Globex Corporation",
    "slug": "globex"
  },
  "message": "Tenant created successfully."
}
Get Tenant
GET /api/tenants/{id}

Example:

GET /api/tenants/1
Authentication
Login
POST /api/auth/login

Required header:

X-Tenant-Id: 1

Request:

{
  "email": "admin@minisaas.com",
  "password": "Admin@123456"
}

Successful response:

{
  "success": true,
  "data": {
    "accessToken": "JWT_TOKEN",
    "expiresAt": "2026-08-16T22:00:00Z"
  },
  "message": "Login successful."
}
Users

All user endpoints require:

X-Tenant-Id: <tenantId>
Authorization: Bearer <token>
Get Users
GET /api/users?pageNumber=1&pageSize=10

Example:

GET /api/users?pageNumber=1&pageSize=10
X-Tenant-Id: 1
Authorization: Bearer <token>
Create User
POST /api/users

Headers:

X-Tenant-Id: 1
Authorization: Bearer <token>
Content-Type: application/json

Request:

{
  "fullName": "Omar Ali",
  "email": "omar@example.com",
  "role": 1,
  "password": "Password@123"
}

The password is hashed before being stored in the database.

Update User
PUT /api/users/{id}

Example:

PUT /api/users/1

Request:

{
  "fullName": "Omar Ali Updated",
  "email": "omar.updated@example.com",
  "role": 2
}
Delete User
DELETE /api/users/{id}

The delete operation is implemented as a soft delete.

The user remains in the database but:

IsActive = false
Error Handling

The API uses a consistent result structure.

Successful response:

{
  "success": true,
  "data": {},
  "message": "Operation completed successfully.",
  "errorCode": 0,
  "errors": null
}

Error response:

{
  "success": false,
  "data": null,
  "message": "User not found.",
  "errorCode": 1001,
  "errors": null
}

Validation errors may contain multiple messages:

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
Validation

The project uses FluentValidation.

Validation is performed before executing business logic.

Example:

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
Hangfire

Hangfire is used for processing background jobs.

The project includes a recurring job that calculates the number of active users for every tenant.

Example:

Tenant 1 → 4 active users
Tenant 2 → 10 active users
Tenant 3 → 0 active users

The job runs independently from HTTP requests.

This is useful when operations are:

Scheduled
Periodic
Long-running
Not required to block an HTTP request
Active Users Background Job

The background job queries tenants and calculates active users.

Conceptually:

SELECT TenantId, COUNT(*)
FROM Users
WHERE IsActive = 1
GROUP BY TenantId

The job logs information such as:

Active users job started.


Tenant 1 has 4 active users.
Tenant 5 has 0 active users.


Active users job completed.
Hangfire Dashboard

Hangfire Dashboard provides visibility into:

Enqueued jobs
Processing jobs
Scheduled jobs
Recurring jobs
Failed jobs
Succeeded jobs
Servers
Retries

During development, the dashboard can be accessed through:

https://localhost:7093/hangfire

or:

http://localhost:5023/hangfire

depending on the selected application URL.

Concurrency Considerations

The application considers concurrency in several areas.

Database

Entity Framework Core handles database operations asynchronously.

await _unitOfWork.SaveChangesAsync(cancellationToken);
Hangfire

Hangfire manages background job execution and server coordination.

Multiple workers can process jobs concurrently.

Tenant Isolation

Tenant context is scoped to the current request and is not shared between requests.

Duplicate Data

The application checks for existing tenant slugs and user emails before inserting records.

Database-level unique constraints should also be used to protect against race conditions where two requests execute simultaneously.

Database

The project uses:

Microsoft SQL Server

Entity Framework Core manages:

Database schema
Migrations
Relationships
Query filters
Persistence
Migrations

Create a migration:

dotnet ef migrations add InitialCreate

Apply migrations:

dotnet ef database update

The application also applies pending migrations during startup.

Database Seeding

The application includes automatic database seeding.

A demo tenant and administrator account are created when the database does not contain the required data.

Default demo tenant:

Name: Demo Tenant
Slug: demo-tenant

Default administrator:

Full Name: System Admin
Email: admin@minisaas.com
Password: Admin@123456
Role: Admin
Tenant ID: 1

For production environments, replace the demo credentials with secure credentials stored outside source control.

Running the Project
Prerequisites

Install:

.NET 10 SDK
SQL Server
Visual Studio 2022 / Visual Studio Code
Postman (optional)
Clone the Repository
git clone <YOUR_GITHUB_REPOSITORY_URL>

Navigate to the project:

cd MiniSaaS
Configure Database

Update the connection string in:

appsettings.json

Example:

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=MiniSaaSDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}

Use your own SQL Server configuration.

Configure JWT

Example configuration:

{
  "Jwt": {
    "Issuer": "MiniSaaS",
    "Audience": "MiniSaaS.Client",
    "SecretKey": "YOUR_SECRET_KEY"
  }
}

For production:

Do not commit real secrets.
Use environment variables.
Use User Secrets for local development.
Use a secure secret manager in production.
Run the Application

From the API project:

dotnet run

The application will start on URLs similar to:

https://localhost:7093
http://localhost:5023
Swagger

Swagger/OpenAPI is enabled for API documentation.

Open:

https://localhost:7093/swagger

or:

http://localhost:5023/swagger

Swagger can be used to:

Explore endpoints
View request models
View response models
Test API endpoints
Authorize using JWT Bearer tokens
Using Swagger with JWT

First call:

POST /api/auth/login

with:

X-Tenant-Id: 1

and:

{
  "email": "admin@minisaas.com",
  "password": "Admin@123456"
}

Copy the returned JWT.

Then click:

Authorize

Enter:

Bearer YOUR_TOKEN

After authorization, protected endpoints can be tested directly from Swagger.

Postman

A Postman collection is included in the repository:

MiniSaaS.postman_collection.json

The collection contains examples for:

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

Import the collection into Postman.

Set:

BaseUrl

to your local API URL.

Example:

https://localhost:7093
Testing

The project uses:

xUnit
Moq

Unit tests cover important business scenarios.

Examples:

Tenant Service
Create tenant successfully
Create tenant with invalid data
Create tenant with duplicate slug
Get existing tenant
Get non-existing tenant
User Service
Get users
Create user successfully
Create user without tenant context
Create user with duplicate email
Password hashing
Update user
Update non-existing user
Update with duplicate email
Delete user
Delete non-existing user
Auth Service
Login successfully
Login without tenant context
User does not exist
Invalid password
JWT token generation
Controllers

Controller tests verify:

HTTP status codes
Service invocation
Successful responses
Error responses
Created responses
Not found responses
Example Unit Test Structure

Tests use Moq to isolate dependencies.

Example:

var unitOfWork = new Mock<IUnitOfWork>();
var tenantRepository = new Mock<IRepository<Tenant>>();


unitOfWork
    .Setup(x => x.Repository<Tenant>())
    .Returns(tenantRepository.Object);

The goal is to test business logic without requiring a real database.

Project Principles

The project follows several important backend development principles:

Separation of Concerns

Each layer has a specific responsibility.

Dependency Inversion

Application services depend on abstractions rather than infrastructure implementations.

Async Programming

Database and background operations use asynchronous APIs.

DTOs

Entities are not directly exposed through API contracts.

Result Pattern

Application operations return a consistent ResultDto<T>.

Tenant Isolation

Tenant-aware data is automatically scoped to the current tenant.

Secure Authentication

Passwords are hashed and JWT tokens are used for authentication.

Soft Delete

Users are deactivated instead of physically removed.

Request Flow

A typical authenticated user request follows this flow:

Client
  │
  │ X-Tenant-Id
  │ Authorization: Bearer JWT
  ▼
ASP.NET Core Middleware
  │
  ├── Authentication
  │
  ├── Authorization
  │
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
Authentication Flow
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
  │
  ├── Find User
  │
  ├── Verify Password
  │
  └── Generate JWT
  │
  ▼
Client
  │
  └── JWT Access Token
Multi-Tenant Request Flow
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
Background Job Flow
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
Security Notes

For production environments:

Replace demo credentials.
Use a strong JWT secret.
Store secrets outside source control.
Use HTTPS.
Add proper authorization policies.
Add rate limiting.
Add refresh tokens if required.
Add audit logging.
Add database unique constraints.
Protect the Hangfire Dashboard.
Avoid exposing sensitive information in logs.
Validate all incoming requests.
Consider tenant authorization based on authenticated identity instead of trusting only a client-supplied tenant header.
Future Improvements

Possible future enhancements:

Refresh Tokens
Email Verification
Password Reset
Admin-only tenant management
Tenant registration flow
Subscription management
Billing integration
Audit Logs
Rate Limiting
API Versioning
Redis caching
Distributed locking
Health Checks
Centralized logging
OpenTelemetry
Docker support
CI/CD pipeline
Integration tests
Testcontainers
Role/Permission management
Tenant-based configuration
Tenant usage limits
Repository Structure
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
├── MiniSaaS.postman_collection.json
├── README.md
└── MiniSaaS.sln
Example Credentials

For local development only:

Tenant:
ID: 1
Name: Demo Tenant
Slug: demo-tenant


Admin:
Email: admin@minisaas.com
Password: Admin@123456
Role: Admin

Use the tenant header:

X-Tenant-Id: 1
License

This project is intended for learning, demonstration, and portfolio purposes.

Author

Built as a backend SaaS architecture project using modern ASP.NET Core technologies.
