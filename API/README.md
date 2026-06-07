# Q2 Invoice Management System

A clean-architecture .NET 10 API for managing invoices and payments, with Keycloak-based JWT authentication.

---

## Solution Structure

```
Q2.slnx
├── API/                  # ASP.NET Core Web API (controllers, extensions, startup)
├── Application/          # Use cases, interfaces, DTOs
├── Domain/               # Entities, value objects, enums (core business logic)
├── Infrastructure/       # EF Core, DB context, configurations, migrations
└── Shared/               # Shared utilities and cross-cutting concerns
```

### Layer Responsibilities

| Layer | Responsibility |
|---|---|
| **Domain** | Entities (`Invoice`, `Payment`, `AppUser`), value objects (`Money`), enums |
| **Application** | Business use cases, repository interfaces, application services |
| **Infrastructure** | EF Core `AppDbContext`, entity configurations, SQL Server migrations |
| **API** | HTTP controllers, JWT middleware, OpenAPI, claim extensions |
| **Shared** | Shared models or helpers referenced across layers |

---

## Domain Model

### Invoice
- Identified by `Guid`
- Owns a collection of `InvoiceItem` (quantity × price)
- Tracks `TotalAmount` and `PaidAmount` as `Money` value objects
- Status transitions: `Pending → PartiallyPaid → Paid`, or `Pending/PartiallyPaid → Cancelled`, or `Pending → Overdue`

### Payment
- Records a payment against an invoice
- Holds `Amount` (Money), `PaymentDate`, `ReferenceNumber`, and optional `Notes`
- Status: `Pending | Completed | Failed | Reversed`

### Money (Value Object)
- Wraps `decimal Amount`
- Enforces non-negative values
- Supports `Add`, `Subtract`, `+`, `-` operators

---

## Invoice Status Flow

```
Pending ──► PartiallyPaid ──► Paid
   │
   ├──► Overdue
   │
   └──► Cancelled
```

Status is managed automatically by `RegisterPayment()` and `MarkAsOverdue()` on the `Invoice` aggregate.

---

## Technology Stack

| Concern | Technology |
|---|---|
| Framework | ASP.NET Core 10 |
| ORM | Entity Framework Core 10 (SQL Server) |
| Authentication | Keycloak (JWT / OpenID Connect) |
| Database | SQL Server |
| API Docs | ASP.NET Core OpenAPI |

---

## Getting Started

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- SQL Server (local or Docker)
- Keycloak instance (local or remote)

### 1. Configure the database connection

Edit `API/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=Q2_2026;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

### 2. Configure Keycloak authentication

In `API/Program.cs`, update the authority and audience to match your Keycloak realm:

```csharp
options.Authority = "http://localhost:8080/realms/YourRealm";
options.Audience  = "your-client-id";
```

### 3. Apply database migrations

```bash
dotnet ef database update --project Infrastructure --startup-project API
```

### 4. Run the API

```bash
dotnet run --project API
```

The API will be available at:
- HTTP: `http://localhost:5036`
- HTTPS: `https://localhost:7249`

OpenAPI docs (development only): `http://localhost:5036/openapi`

---

## Authentication

The API uses **Keycloak** as the identity provider. All protected endpoints require a valid JWT Bearer token.

Useful claim helpers are available via `UserExtensions`:

```csharp
user.GetKeycloakUserId()   // "sub" claim
user.GetEmail()            // "email" claim
user.GetUserName()         // "preferred_username" claim
```

---

## Key Business Rules

- An invoice's due date cannot be in the past at creation time.
- Items cannot be added to or removed from a **Paid** invoice.
- A **Cancelled** invoice cannot receive payments.
- A **Paid** invoice cannot be cancelled.
- `PaidAmount` is updated via `RegisterPayment(decimal amount)`; status recalculates automatically.
- `MarkAsOverdue()` transitions a `Pending` invoice to `Overdue` when past its due date.
- `Money` values are always non-negative; subtraction below zero throws an exception.

---

## Database Schema

| Table | Description |
|---|---|
| `Invoices` | Invoice header with status, amounts, customer, and due date |
| `InvoiceItem` | Line items owned by an invoice (name, price, quantity) |
| `Lookups` | Generic key-value lookup table (name, code, value) |
| `Payments` | Payment records linked to invoices |
| `Users` | Application users synced from Keycloak |

---

## Project Dependencies

```
API  →  Application, Infrastructure
Infrastructure  →  Application, Domain
Application  →  Domain
Shared  →  Domain
```
