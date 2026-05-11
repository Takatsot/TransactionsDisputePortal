# Transactions Dispute Portal - Planning Document
## Senior-Level Interview Assessment

**Date:** May 4, 2026  
**Version:** 1.0  
**Author:** Senior Software Architect

---

## Table of Contents

1. [System Overview](#1-system-overview)
2. [Architecture Design](#2-architecture-design)
3. [Technology Stack](#3-technology-stack)
4. [Data Model Design](#4-data-model-design)
5. [API Design](#5-api-design)
6. [Frontend Design](#6-frontend-design)
7. [Business Logic](#7-business-logic)
8. [Security Considerations](#8-security-considerations)
9. [Performance & Scalability](#9-performance--scalability)
10. [Testing Strategy](#10-testing-strategy)
11. [Deployment Approach](#11-deployment-approach)
12. [Optional Enhancements](#12-optional-enhancements)

---

## 1. System Overview

### 1.1 Purpose

The Transactions Dispute Portal is a comprehensive system that enables customers to:
- View their financial transactions in a clear, organized manner
- Initiate disputes for questionable or unauthorized transactions
- Track the status of their disputes through a complete lifecycle
- View historical records of all disputed transactions

The system demonstrates enterprise-level software development practices, including Clean Architecture, SOLID principles, CQRS pattern, and scalable design suitable for production environments.

### 1.2 Key Features

#### Customer Features
1. **Transaction Management**
   - View paginated list of all transactions
   - Filter transactions by date range, amount, merchant, status
   - Search transactions by description or merchant name
   - Sort by date, amount, or merchant

2. **Dispute Management**
   - Create disputes for specific transactions
   - Provide dispute reason and supporting details
   - Upload supporting documents (optional enhancement)
   - Cancel pending disputes
   
3. **Dispute Tracking**
   - View dispute status in real-time
   - Access complete dispute history
   - Receive status update notifications
   - View resolution details and notes

4. **Dashboard & Analytics**
   - Summary of total transactions
   - Count of pending/resolved/rejected disputes
   - Quick access to recent disputes
   - Visual representation of dispute trends

### 1.3 User Flows

#### Flow 1: View Transactions
```
User Login → Dashboard → Transactions List → Filter/Search → View Transaction Details
```

#### Flow 2: Create Dispute
```
Transaction List → Select Transaction → Click "Dispute" → 
Fill Dispute Form (Reason, Description, Evidence) → Submit → 
Confirmation → View Dispute Details
```

#### Flow 3: Track Dispute
```
Dashboard → My Disputes → Select Dispute → View Status & History → 
View Resolution (if completed)
```

#### Flow 4: Filter Dispute History
```
My Disputes → Apply Filters (Status, Date Range) → 
View Filtered Results → Export (optional)
```

---

## 2. Architecture Design

### 2.1 High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      PRESENTATION LAYER                      │
│  ┌──────────────────────┐      ┌──────────────────────┐    │
│  │   React Frontend     │◄────►│   ASP.NET Core API   │    │
│  │  (SPA Application)   │ HTTP │   (REST Endpoints)   │    │
│  └──────────────────────┘      └──────────────────────┘    │
└────────────────────────────────────┬────────────────────────┘
                                     │
┌────────────────────────────────────┼────────────────────────┐
│               APPLICATION LAYER    │                         │
│  ┌──────────────────────────────────────────────────────┐  │
│  │              MediatR Pipeline                         │  │
│  │  ┌────────────┐  ┌──────────────┐  ┌─────────────┐  │  │
│  │  │  Commands  │  │   Queries    │  │  Handlers   │  │  │
│  │  │  (CQRS)    │  │   (CQRS)     │  │             │  │  │
│  │  └────────────┘  └──────────────┘  └─────────────┘  │  │
│  └──────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │    Pipeline Behaviors (Cross-Cutting Concerns)        │  │
│  │  • Validation  • Logging  • Authorization  • UoW     │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────────────────┬────────────────────────┘
                                     │
┌────────────────────────────────────┼────────────────────────┐
│                 DOMAIN LAYER       │                         │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐     │
│  │   Entities   │  │  Value Objs  │  │ Domain Events│     │
│  │  • Transaction│  │  • Money     │  │              │     │
│  │  • Dispute    │  │  • Status    │  │              │     │
│  │  • Customer   │  │              │  │              │     │
│  └──────────────┘  └──────────────┘  └──────────────┘     │
│  ┌──────────────────────────────────────────────────────┐  │
│  │    Domain Services & Business Rules                  │  │
│  └──────────────────────────────────────────────────────┘  │
└────────────────────────────────────┬────────────────────────┘
                                     │
┌────────────────────────────────────┼────────────────────────┐
│            INFRASTRUCTURE LAYER    │                         │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  Entity Framework Core  (DbContext, Repositories)    │  │
│  └──────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  External Services (Email, Storage, Caching)         │  │
│  └──────────────────────────────────────────────────────┘  │
│  ┌──────────────────────────────────────────────────────┐  │
│  │  SQL Server / PostgreSQL Database                    │  │
│  └──────────────────────────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
```

### 2.2 Clean Architecture Pattern

The system follows **Clean Architecture** (Onion Architecture) principles:

#### Layer Responsibilities

**1. Domain Layer (Core)** - `TransactionsDisputePortal.Domain`
- **Contains:** Entities, Value Objects, Enums, Domain Events, Exceptions, Interfaces
- **Depends on:** Nothing (Pure domain logic)
- **Purpose:** Encapsulate enterprise business rules
- **SOLID Principles:**
  - Single Responsibility: Each entity has one reason to change
  - Open/Closed: Extensible through domain events
  - Liskov Substitution: Value objects are immutable and substitutable
  - Interface Segregation: Repository interfaces specific to needs
  - Dependency Inversion: No dependencies on outer layers

**2. Application Layer** - `TransactionsDisputePortal.Application`
- **Contains:** Commands, Queries, DTOs, Validators, Mappers, Pipeline Behaviors
- **Depends on:** Domain Layer only
- **Purpose:** Orchestrate application use cases (CQRS pattern)
- **Patterns Implemented:**
  - **CQRS:** Separate Commands (write) and Queries (read)
  - **Mediator Pattern:** MediatR for decoupling
  - **Decorator Pattern:** Pipeline behaviors for cross-cutting concerns
  - **Repository Pattern:** Abstract data access

**3. Infrastructure Layer** - `TransactionsDisputePortal.Infrastructure`
- **Contains:** EF Core implementation, Repositories, External Services, Migrations
- **Depends on:** Application and Domain layers
- **Purpose:** Implement infrastructure concerns
- **Responsibilities:**
  - Database access via Entity Framework Core
  - External service integrations
  - File storage
  - Caching implementation

**4. Presentation Layer (API)** - `TransactionsDisputePortal.Api`
- **Contains:** Controllers, Filters, Middleware, Configuration
- **Depends on:** Application layer (and Infrastructure for DI)
- **Purpose:** HTTP API endpoints and request/response handling
- **Features:**
  - RESTful API design
  - API versioning
  - Swagger/OpenAPI documentation
  - Global exception handling
  - Authentication/Authorization

**5. Frontend (React SPA)** - `transactions-dispute-portal-ui`
- **Contains:** React components, state management, API clients
- **Depends on:** API layer via HTTP
- **Purpose:** User interface and experience

### 2.3 Component Responsibilities

```
┌─────────────────────────────────────────────────────────┐
│                    API Controllers                       │
│  • Route HTTP requests to MediatR                       │
│  • Handle API versioning                                │
│  • Return standardized responses                        │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                  MediatR Pipeline                        │
│  Commands/Queries → Behaviors → Handlers                │
└────────────────────┬────────────────────────────────────┘
                     │
        ┌────────────┴───────────┐
        │                        │
┌───────▼────────┐      ┌────────▼────────┐
│   Validators   │      │    AutoMapper    │
│ FluentValidation│     │  DTO Mapping    │
└───────┬────────┘      └────────┬────────┘
        │                        │
        └────────────┬───────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                  Domain Services                         │
│  Business logic, validation rules, domain events        │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                  Repositories                            │
│  Data access abstraction                                │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│               Entity Framework Core                      │
│  ORM, Change Tracking, Database Access                  │
└────────────────────┬────────────────────────────────────┘
                     │
                ┌────▼────┐
                │Database │
                └─────────┘
```

### 2.4 Design Patterns Applied

1. **CQRS (Command Query Responsibility Segregation)**
   - Commands: Create/Update/Delete operations
   - Queries: Read operations with optimized DTOs

2. **Mediator Pattern**
   - Decouple request handling from controllers
   - Centralized pipeline for cross-cutting concerns

3. **Repository Pattern**
   - Abstract data access layer
   - Enable unit testing with mock repositories

4. **Unit of Work Pattern**
   - Manage database transactions
   - Ensure data consistency

5. **Specification Pattern** (Optional Enhancement)
   - Encapsulate query logic
   - Reusable query specifications

6. **Factory Pattern**
   - Create complex domain objects
   - Ensure valid object initialization

7. **Decorator Pattern**
   - Pipeline behaviors wrap handlers
   - Add functionality without modifying handlers

---

## 3. Technology Stack

### 3.1 Backend Technologies

#### Core Framework
- **ASP.NET Core 8.0** (or latest LTS)
  - Web API with minimal API support
  - Built-in dependency injection
  - High performance and cross-platform

#### Libraries & NuGet Packages

**Application Layer**
```xml
<PackageReference Include="MediatR" Version="12.x" />
<PackageReference Include="FluentValidation" Version="11.x" />
<PackageReference Include="FluentValidation.DependencyInjectionExtensions" Version="11.x" />
<PackageReference Include="AutoMapper" Version="12.x" />
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.x" />
```

**Infrastructure Layer**
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.x" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.x" />
<!-- OR -->
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.x" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.x" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.x" />
```

**API Layer**
```xml
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.x" />
<PackageReference Include="Serilog.AspNetCore" Version="8.x" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.x" />
<PackageReference Include="Serilog.Sinks.File" Version="5.x" />
<PackageReference Include="Microsoft.AspNetCore.Authentication.JwtBearer" Version="8.x" />
<PackageReference Include="AspNetCore.HealthChecks.UI" Version="7.x" />
```

**Caching (Optional)**
```xml
<PackageReference Include="Microsoft.Extensions.Caching.Memory" Version="8.x" />
<PackageReference Include="Microsoft.Extensions.Caching.StackExchangeRedis" Version="8.x" />
```

### 3.2 Frontend Technologies (React)

#### Core Framework
```json
{
  "dependencies": {
    "react": "^18.2.0",
    "react-dom": "^18.2.0",
    "react-router-dom": "^6.x"
  }
}
```

#### State Management
```json
{
  "dependencies": {
    "@tanstack/react-query": "^5.x",  // Server state management
    "zustand": "^4.x"                  // Client state management
  }
}
```
**Alternative:** Redux Toolkit, Recoil, or Context API

#### UI Component Library
```json
{
  "dependencies": {
    "@mui/material": "^5.x",          // Material-UI
    "@mui/x-data-grid": "^6.x",       // Data tables
    "@emotion/react": "^11.x",
    "@emotion/styled": "^11.x"
  }
}
```
**Alternatives:** Ant Design, Chakra UI, Tailwind CSS + Headless UI

#### HTTP Client
```json
{
  "dependencies": {
    "axios": "^1.x"
  }
}
```

#### Form Handling & Validation
```json
{
  "dependencies": {
    "react-hook-form": "^7.x",
    "yup": "^1.x" or "zod": "^3.x"
  }
}
```

#### Utilities
```json
{
  "dependencies": {
    "date-fns": "^2.x",              // Date manipulation
    "react-toastify": "^9.x",        // Notifications
    "react-icons": "^4.x",           // Icons
    "chart.js": "^4.x",              // Charts (optional)
    "react-chartjs-2": "^5.x"
  }
}
```

#### Development Tools
```json
{
  "devDependencies": {
    "@types/react": "^18.x",
    "@types/react-dom": "^18.x",
    "@typescript-eslint/eslint-plugin": "^6.x",
    "@typescript-eslint/parser": "^6.x",
    "typescript": "^5.x",
    "vite": "^5.x",                  // Build tool
    "@vitejs/plugin-react": "^4.x",
    "eslint": "^8.x",
    "prettier": "^3.x"
  }
}
```

### 3.3 Database Choice

#### Primary Recommendation: **SQL Server**
**Pros:**
- Native integration with .NET ecosystem
- Excellent tooling (SSMS, Azure Data Studio)
- Strong support for transactions and ACID properties
- Built-in full-text search
- LocalDB for development

**Connection String:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TransactionsDisputePortalDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

#### Alternative: **PostgreSQL**
**Pros:**
- Open-source and free
- Excellent performance
- Strong community support
- JSON support for flexible data

**Connection String:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=transactionsdisputeportal;Username=postgres;Password=yourpassword"
  }
}
```

### 3.4 Supporting Tools & Libraries

#### Development Tools
- **Visual Studio 2022** or **JetBrains Rider** - Backend development
- **Visual Studio Code** - Frontend development
- **Postman** or **Insomnia** - API testing
- **SQL Server Management Studio** or **Azure Data Studio** - Database management

#### CI/CD & DevOps
- **GitHub Actions** or **Azure DevOps** - CI/CD pipelines
- **Docker** - Containerization
- **Docker Compose** - Local multi-container setup

#### Monitoring & Logging
- **Serilog** - Structured logging
- **Application Insights** (Azure) - Application monitoring
- **Health Checks** - Application health monitoring

---

## 4. Data Model Design

### 4.1 Entity Relationship Diagram

```
┌─────────────────────────┐
│       Customer          │
├─────────────────────────┤
│ Id (PK, Guid)           │
│ Email (string)          │
│ FirstName (string)      │
│ LastName (string)       │
│ CreatedDate (DateTime)  │
│ IsActive (bool)         │
└────────┬────────────────┘
         │ 1
         │
         │ *
┌────────▼────────────────┐
│     Transaction         │
├─────────────────────────┤
│ Id (PK, Guid)           │
│ CustomerId (FK)         │───┐
│ TransactionDate         │   │
│ Amount (decimal)        │   │
│ Currency (string)       │   │
│ MerchantName (string)   │   │
│ Description (string)    │   │
│ Category (string)       │   │
│ TransactionType (enum)  │   │
│ Status (enum)           │   │
│ CreatedDate (DateTime)  │   │
│ UpdatedDate (DateTime)  │   │
└────────┬────────────────┘   │
         │ 1                  │
         │                    │
         │ 0..1               │
┌────────▼────────────────┐   │
│       Dispute           │   │
├─────────────────────────┤   │
│ Id (PK, Guid)           │   │
│ TransactionId (FK)      │───┘
│ CustomerId (FK)         │───┐
│ DisputeReason (enum)    │   │
│ Description (string)    │   │
│ Status (enum)           │   │
│ CreatedDate (DateTime)  │   │
│ UpdatedDate (DateTime)  │   │
│ ResolvedDate (DateTime?)│   │
│ ResolutionNotes (string)│   │
└────────┬────────────────┘   │
         │ 1                  │
         │                    │
         │ *                  │
┌────────▼────────────────┐   │
│   DisputeHistory        │   │
├─────────────────────────┤   │
│ Id (PK, Guid)           │   │
│ DisputeId (FK)          │   │
│ Status (enum)           │   │
│ Notes (string)          │   │
│ ChangedBy (string)      │   │
│ ChangedDate (DateTime)  │   │
└─────────────────────────┘   │
                              │
         ┌────────────────────┘
         │
         │
┌────────▼────────────────┐
│  DisputeEvidence        │
├─────────────────────────┤
│ Id (PK, Guid)           │
│ DisputeId (FK)          │
│ FileName (string)       │
│ FileUrl (string)        │
│ FileType (string)       │
│ UploadedDate (DateTime) │
└─────────────────────────┘
```

### 4.2 Entity Details

#### 4.2.1 Customer Entity

```csharp
namespace TransactionsDisputePortal.Domain.Entities
{
    public class Customer
    {
        public Guid Id { get; private set; }
        public string Email { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public bool IsActive { get; private set; }
        
        // Navigation properties
        public virtual ICollection<Transaction> Transactions { get; private set; }
        public virtual ICollection<Dispute> Disputes { get; private set; }
        
        // Computed properties
        public string FullName => $"{FirstName} {LastName}";
        
        // Factory method
        public static Customer Create(string email, string firstName, string lastName)
        {
            // Validation logic
            return new Customer
            {
                Id = Guid.NewGuid(),
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                Transactions = new List<Transaction>(),
                Disputes = new List<Dispute>()
            };
        }
        
        // Domain methods
        public void Deactivate() => IsActive = false;
        public void Activate() => IsActive = true;
    }
}
```

**Validation Rules:**
- Email: Required, valid email format, unique
- FirstName: Required, max 50 characters
- LastName: Required, max 50 characters

#### 4.2.2 Transaction Entity

```csharp
namespace TransactionsDisputePortal.Domain.Entities
{
    public class Transaction
    {
        public Guid Id { get; private set; }
        public Guid CustomerId { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        public string MerchantName { get; private set; }
        public string Description { get; private set; }
        public string Category { get; private set; }
        public TransactionType Type { get; private set; }
        public TransactionStatus Status { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; private set; }
        
        // Navigation properties
        public virtual Customer Customer { get; private set; }
        public virtual Dispute? Dispute { get; private set; }
        
        // Computed properties
        public bool IsDisputed => Dispute != null;
        public bool CanBeDisputed => 
            Dispute == null && 
            TransactionDate >= DateTime.UtcNow.AddDays(-90) &&
            Status == TransactionStatus.Completed;
        
        // Factory method
        public static Transaction Create(
            Guid customerId,
            DateTime transactionDate,
            decimal amount,
            string currency,
            string merchantName,
            string description,
            string category,
            TransactionType type)
        {
            return new Transaction
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                TransactionDate = transactionDate,
                Amount = amount,
                Currency = currency,
                MerchantName = merchantName,
                Description = description,
                Category = category,
                Type = type,
                Status = TransactionStatus.Completed,
                CreatedDate = DateTime.UtcNow
            };
        }
        
        // Domain methods
        public void MarkAsDisputed()
        {
            if (!CanBeDisputed)
                throw new InvalidOperationException("Transaction cannot be disputed");
            
            Status = TransactionStatus.Disputed;
            UpdatedDate = DateTime.UtcNow;
        }
    }
}
```

**Validation Rules:**
- Amount: Required, greater than 0, max 2 decimal places
- Currency: Required, ISO 4217 code (USD, EUR, etc.)
- MerchantName: Required, max 100 characters
- Description: Optional, max 500 characters
- TransactionDate: Cannot be in the future

#### 4.2.3 Dispute Entity

```csharp
namespace TransactionsDisputePortal.Domain.Entities
{
    public class Dispute
    {
        public Guid Id { get; private set; }
        public Guid TransactionId { get; private set; }
        public Guid CustomerId { get; private set; }
        public DisputeReason Reason { get; private set; }
        public string Description { get; private set; }
        public DisputeStatus Status { get; private set; }
        public DateTime CreatedDate { get; private set; }
        public DateTime? UpdatedDate { get; private set; }
        public DateTime? ResolvedDate { get; private set; }
        public string? ResolutionNotes { get; private set; }
        
        // Navigation properties
        public virtual Transaction Transaction { get; private set; }
        public virtual Customer Customer { get; private set; }
        public virtual ICollection<DisputeHistory> History { get; private set; }
        public virtual ICollection<DisputeEvidence> Evidence { get; private set; }
        
        // Computed properties
        public bool IsActive => Status == DisputeStatus.Pending || 
                                Status == DisputeStatus.UnderReview;
        public TimeSpan? ResolutionTime => ResolvedDate.HasValue ? 
            ResolvedDate.Value - CreatedDate : null;
        
        // Factory method
        public static Dispute Create(
            Guid transactionId,
            Guid customerId,
            DisputeReason reason,
            string description)
        {
            var dispute = new Dispute
            {
                Id = Guid.NewGuid(),
                TransactionId = transactionId,
                CustomerId = customerId,
                Reason = reason,
                Description = description,
                Status = DisputeStatus.Pending,
                CreatedDate = DateTime.UtcNow,
                History = new List<DisputeHistory>(),
                Evidence = new List<DisputeEvidence>()
            };
            
            // Add initial history entry
            dispute.AddHistoryEntry(DisputeStatus.Pending, "Dispute created", "System");
            
            return dispute;
        }
        
        // Domain methods
        public void Approve(string notes, string approvedBy)
        {
            if (!IsActive)
                throw new InvalidOperationException("Only active disputes can be approved");
            
            Status = DisputeStatus.Approved;
            ResolutionNotes = notes;
            ResolvedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
            
            AddHistoryEntry(DisputeStatus.Approved, notes, approvedBy);
        }
        
        public void Reject(string notes, string rejectedBy)
        {
            if (!IsActive)
                throw new InvalidOperationException("Only active disputes can be rejected");
            
            Status = DisputeStatus.Rejected;
            ResolutionNotes = notes;
            ResolvedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
            
            AddHistoryEntry(DisputeStatus.Rejected, notes, rejectedBy);
        }
        
        public void MarkUnderReview(string notes, string reviewedBy)
        {
            if (Status != DisputeStatus.Pending)
                throw new InvalidOperationException("Only pending disputes can be marked under review");
            
            Status = DisputeStatus.UnderReview;
            UpdatedDate = DateTime.UtcNow;
            
            AddHistoryEntry(DisputeStatus.UnderReview, notes, reviewedBy);
        }
        
        public void Cancel(string reason, string cancelledBy)
        {
            if (!IsActive)
                throw new InvalidOperationException("Only active disputes can be cancelled");
            
            Status = DisputeStatus.Cancelled;
            ResolutionNotes = reason;
            ResolvedDate = DateTime.UtcNow;
            UpdatedDate = DateTime.UtcNow;
            
            AddHistoryEntry(DisputeStatus.Cancelled, reason, cancelledBy);
        }
        
        private void AddHistoryEntry(DisputeStatus status, string notes, string changedBy)
        {
            History.Add(DisputeHistory.Create(Id, status, notes, changedBy));
        }
    }
}
```

**Validation Rules:**
- Reason: Required enum value
- Description: Required, min 20 characters, max 1000 characters
- ResolutionNotes: Optional, max 2000 characters

#### 4.2.4 DisputeHistory Entity

```csharp
namespace TransactionsDisputePortal.Domain.Entities
{
    public class DisputeHistory
    {
        public Guid Id { get; private set; }
        public Guid DisputeId { get; private set; }
        public DisputeStatus Status { get; private set; }
        public string Notes { get; private set; }
        public string ChangedBy { get; private set; }
        public DateTime ChangedDate { get; private set; }
        
        // Navigation properties
        public virtual Dispute Dispute { get; private set; }
        
        // Factory method
        public static DisputeHistory Create(
            Guid disputeId,
            DisputeStatus status,
            string notes,
            string changedBy)
        {
            return new DisputeHistory
            {
                Id = Guid.NewGuid(),
                DisputeId = disputeId,
                Status = status,
                Notes = notes,
                ChangedBy = changedBy,
                ChangedDate = DateTime.UtcNow
            };
        }
    }
}
```

#### 4.2.5 DisputeEvidence Entity (Optional Enhancement)

```csharp
namespace TransactionsDisputePortal.Domain.Entities
{
    public class DisputeEvidence
    {
        public Guid Id { get; private set; }
        public Guid DisputeId { get; private set; }
        public string FileName { get; private set; }
        public string FileUrl { get; private set; }
        public string FileType { get; private set; }
        public long FileSize { get; private set; }
        public DateTime UploadedDate { get; private set; }
        
        // Navigation properties
        public virtual Dispute Dispute { get; private set; }
        
        // Factory method
        public static DisputeEvidence Create(
            Guid disputeId,
            string fileName,
            string fileUrl,
            string fileType,
            long fileSize)
        {
            return new DisputeEvidence
            {
                Id = Guid.NewGuid(),
                DisputeId = disputeId,
                FileName = fileName,
                FileUrl = fileUrl,
                FileType = fileType,
                FileSize = fileSize,
                UploadedDate = DateTime.UtcNow
            };
        }
    }
}
```

### 4.3 Enumerations

#### TransactionType
```csharp
public enum TransactionType
{
    Debit = 1,
    Credit = 2,
    Refund = 3,
    Fee = 4
}
```

#### TransactionStatus
```csharp
public enum TransactionStatus
{
    Pending = 1,
    Completed = 2,
    Disputed = 3,
    Reversed = 4,
    Failed = 5
}
```

#### DisputeReason
```csharp
public enum DisputeReason
{
    UnauthorizedTransaction = 1,
    IncorrectAmount = 2,
    DuplicateCharge = 3,
    ProductNotReceived = 4,
    ProductDefective = 5,
    ServiceNotProvided = 6,
    Fraudulent = 7,
    Other = 99
}
```

#### DisputeStatus
```csharp
public enum DisputeStatus
{
    Pending = 1,
    UnderReview = 2,
    Approved = 3,
    Rejected = 4,
    Cancelled = 5
}
```

### 4.4 Value Objects

#### Money Value Object
```csharp
namespace TransactionsDisputePortal.Domain.ValueObjects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get; private set; }
        
        private Money() { }
        
        public Money(decimal amount, string currency)
        {
            if (amount < 0)
                throw new ArgumentException("Amount cannot be negative");
            
            if (string.IsNullOrWhiteSpace(currency))
                throw new ArgumentException("Currency is required");
            
            Amount = Math.Round(amount, 2);
            Currency = currency.ToUpper();
        }
        
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }
        
        public static Money operator +(Money a, Money b)
        {
            if (a.Currency != b.Currency)
                throw new InvalidOperationException("Cannot add money with different currencies");
            
            return new Money(a.Amount + b.Amount, a.Currency);
        }
        
        public override string ToString() => $"{Amount:N2} {Currency}";
    }
}
```

### 4.5 Database Indexes

**Performance Optimization Indexes:**

```sql
-- Transaction indexes
CREATE INDEX IX_Transaction_CustomerId ON Transactions(CustomerId);
CREATE INDEX IX_Transaction_TransactionDate ON Transactions(TransactionDate DESC);
CREATE INDEX IX_Transaction_Status ON Transactions(Status);
CREATE INDEX IX_Transaction_MerchantName ON Transactions(MerchantName);

-- Dispute indexes
CREATE INDEX IX_Dispute_CustomerId ON Disputes(CustomerId);
CREATE INDEX IX_Dispute_TransactionId ON Disputes(TransactionId);
CREATE INDEX IX_Dispute_Status ON Disputes(Status);
CREATE INDEX IX_Dispute_CreatedDate ON Disputes(CreatedDate DESC);

-- Composite indexes for common queries
CREATE INDEX IX_Transaction_CustomerId_Date ON Transactions(CustomerId, TransactionDate DESC);
CREATE INDEX IX_Dispute_CustomerId_Status ON Disputes(CustomerId, Status);
```

---

## 5. API Design

### 5.1 API Principles

- **RESTful Design:** Follow REST conventions for resource naming and HTTP methods
- **Consistent Naming:** Use plural nouns for collections (e.g., `/api/transactions`)
- **Versioning:** Use URL versioning (e.g., `/api/v1/transactions`)
- **Status Codes:** Use appropriate HTTP status codes
- **Error Handling:** Consistent error response format (RFC 7807 Problem Details)
- **Pagination:** Use query parameters for pagination
- **Filtering:** Support query parameters for filtering and sorting

### 5.2 Base URL Structure

```
Base URL: https://localhost:7000/api/v1
```

### 5.3 Endpoint Specifications

#### 5.3.1 Authentication Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/v1/auth/login` | Authenticate user |
| POST | `/api/v1/auth/register` | Register new user |
| POST | `/api/v1/auth/refresh` | Refresh access token |
| POST | `/api/v1/auth/logout` | Logout user |

**POST /api/v1/auth/login**

Request:
```json
{
  "email": "john.doe@example.com",
  "password": "SecurePassword123!"
}
```

Response (200 OK):
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "refreshToken": "def50200e3b0c44298fc1c149afbf4c8996fb92427ae41e4649b934ca495991...",
  "expiresIn": 3600,
  "tokenType": "Bearer",
  "user": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "email": "john.doe@example.com",
    "firstName": "John",
    "lastName": "Doe"
  }
}
```

#### 5.3.2 Transaction Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/transactions` | Get paginated list of transactions |
| GET | `/api/v1/transactions/{id}` | Get transaction by ID |
| GET | `/api/v1/transactions/summary` | Get transaction summary/stats |

**GET /api/v1/transactions**

Query Parameters:
```
?pageNumber=1
&pageSize=20
&sortBy=transactionDate
&sortOrder=desc
&searchTerm=amazon
&startDate=2024-01-01
&endDate=2024-12-31
&minAmount=10.00
&maxAmount=500.00
&status=completed
&category=shopping
```

Response (200 OK):
```json
{
  "items": [
    {
      "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "transactionDate": "2024-05-01T14:30:00Z",
      "amount": 125.50,
      "currency": "USD",
      "merchantName": "Amazon.com",
      "description": "Electronics purchase",
      "category": "Shopping",
      "type": "Debit",
      "status": "Completed",
      "isDisputed": false,
      "canBeDisputed": true
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalPages": 5,
  "totalCount": 95,
  "hasPreviousPage": false,
  "hasNextPage": true
}
```

**GET /api/v1/transactions/{id}**

Response (200 OK):
```json
{
  "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "transactionDate": "2024-05-01T14:30:00Z",
  "amount": 125.50,
  "currency": "USD",
  "merchantName": "Amazon.com",
  "description": "Electronics purchase",
  "category": "Shopping",
  "type": "Debit",
  "status": "Completed",
  "isDisputed": false,
  "canBeDisputed": true,
  "dispute": null,
  "createdDate": "2024-05-01T14:30:00Z",
  "updatedDate": null
}
```

Response (404 Not Found):
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.4",
  "title": "Not Found",
  "status": 404,
  "detail": "Transaction with ID '3fa85f64-5717-4562-b3fc-2c963f66afa6' was not found.",
  "traceId": "00-0af7651916cd43dd8448eb211c80319c-b7ad6b7169203331-00"
}
```

**GET /api/v1/transactions/summary**

Response (200 OK):
```json
{
  "totalTransactions": 1250,
  "totalAmount": 45678.90,
  "currency": "USD",
  "disputedCount": 5,
  "pendingDisputesCount": 2,
  "averageTransactionAmount": 36.54,
  "lastTransactionDate": "2024-05-04T10:15:00Z",
  "categoryBreakdown": [
    {
      "category": "Shopping",
      "count": 450,
      "totalAmount": 15678.90
    },
    {
      "category": "Dining",
      "count": 320,
      "totalAmount": 8456.50
    }
  ]
}
```

#### 5.3.3 Dispute Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/disputes` | Get paginated list of disputes |
| GET | `/api/v1/disputes/{id}` | Get dispute by ID |
| POST | `/api/v1/disputes` | Create new dispute |
| PUT | `/api/v1/disputes/{id}/cancel` | Cancel dispute |
| GET | `/api/v1/disputes/{id}/history` | Get dispute history |

**GET /api/v1/disputes**

Query Parameters:
```
?pageNumber=1
&pageSize=20
&status=pending
&sortBy=createdDate
&sortOrder=desc
```

Response (200 OK):
```json
{
  "items": [
    {
      "id": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
      "transactionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
      "transaction": {
        "transactionDate": "2024-05-01T14:30:00Z",
        "amount": 125.50,
        "currency": "USD",
        "merchantName": "Amazon.com",
        "description": "Electronics purchase"
      },
      "reason": "UnauthorizedTransaction",
      "reasonDescription": "Unauthorized Transaction",
      "description": "I did not authorize this purchase. My card was lost.",
      "status": "Pending",
      "statusDescription": "Pending",
      "createdDate": "2024-05-02T09:15:00Z",
      "updatedDate": null,
      "resolvedDate": null,
      "resolutionNotes": null,
      "isActive": true
    }
  ],
  "pageNumber": 1,
  "pageSize": 20,
  "totalPages": 1,
  "totalCount": 5,
  "hasPreviousPage": false,
  "hasNextPage": false
}
```

**GET /api/v1/disputes/{id}**

Response (200 OK):
```json
{
  "id": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
  "transactionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "transaction": {
    "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
    "transactionDate": "2024-05-01T14:30:00Z",
    "amount": 125.50,
    "currency": "USD",
    "merchantName": "Amazon.com",
    "description": "Electronics purchase",
    "category": "Shopping",
    "type": "Debit"
  },
  "reason": "UnauthorizedTransaction",
  "reasonDescription": "Unauthorized Transaction",
  "description": "I did not authorize this purchase. My card was lost.",
  "status": "Pending",
  "statusDescription": "Pending",
  "createdDate": "2024-05-02T09:15:00Z",
  "updatedDate": null,
  "resolvedDate": null,
  "resolutionNotes": null,
  "isActive": true,
  "evidenceCount": 0
}
```

**POST /api/v1/disputes**

Request:
```json
{
  "transactionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "reason": "UnauthorizedTransaction",
  "description": "I did not authorize this purchase. My card was lost on April 30th and this transaction occurred after that date."
}
```

Response (201 Created):
```json
{
  "id": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
  "transactionId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
  "reason": "UnauthorizedTransaction",
  "description": "I did not authorize this purchase. My card was lost on April 30th...",
  "status": "Pending",
  "createdDate": "2024-05-02T09:15:00Z",
  "message": "Dispute created successfully. You will receive updates via email."
}
```

Response (400 Bad Request):
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "TransactionId": [
      "Transaction does not exist."
    ],
    "Description": [
      "Description must be at least 20 characters."
    ]
  },
  "traceId": "00-0af7651916cd43dd8448eb211c80319c-b7ad6b7169203331-00"
}
```

Response (409 Conflict):
```json
{
  "type": "https://tools.ietf.org/html/rfc7231#section-6.5.8",
  "title": "Conflict",
  "status": 409,
  "detail": "This transaction already has an active dispute.",
  "traceId": "00-0af7651916cd43dd8448eb211c80319c-b7ad6b7169203331-00"
}
```

**PUT /api/v1/disputes/{id}/cancel**

Request:
```json
{
  "reason": "Issue resolved with merchant directly"
}
```

Response (200 OK):
```json
{
  "id": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
  "status": "Cancelled",
  "message": "Dispute cancelled successfully."
}
```

**GET /api/v1/disputes/{id}/history**

Response (200 OK):
```json
{
  "disputeId": "7fa85f64-5717-4562-b3fc-2c963f66afa7",
  "history": [
    {
      "id": "9fa85f64-5717-4562-b3fc-2c963f66afa9",
      "status": "Pending",
      "statusDescription": "Pending",
      "notes": "Dispute created",
      "changedBy": "System",
      "changedDate": "2024-05-02T09:15:00Z"
    },
    {
      "id": "afa85f64-5717-4562-b3fc-2c963f66afaa",
      "status": "UnderReview",
      "statusDescription": "Under Review",
      "notes": "Assigned to review team",
      "changedBy": "admin@example.com",
      "changedDate": "2024-05-02T14:30:00Z"
    }
  ]
}
```

#### 5.3.4 Customer Endpoints (Optional)

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/v1/customers/profile` | Get current user profile |
| PUT | `/api/v1/customers/profile` | Update user profile |

### 5.4 Error Response Format (RFC 7807)

All error responses follow the Problem Details specification:

```json
{
  "type": "https://example.com/problems/validation-error",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "detail": "Please check the errors property for additional details.",
  "errors": {
    "PropertyName": [
      "Error message 1",
      "Error message 2"
    ]
  },
  "traceId": "00-0af7651916cd43dd8448eb211c80319c-b7ad6b7169203331-00",
  "timestamp": "2024-05-04T10:30:00Z"
}
```

### 5.5 HTTP Status Codes

| Code | Usage |
|------|-------|
| 200 OK | Successful GET, PUT requests |
| 201 Created | Successful POST creating resource |
| 204 No Content | Successful DELETE |
| 400 Bad Request | Validation errors, malformed request |
| 401 Unauthorized | Authentication required |
| 403 Forbidden | Insufficient permissions |
| 404 Not Found | Resource not found |
| 409 Conflict | Business rule violation (e.g., duplicate dispute) |
| 422 Unprocessable Entity | Semantic errors |
| 500 Internal Server Error | Unexpected server error |

### 5.6 API Controller Example

```csharp
namespace TransactionsDisputePortal.Api.Controllers.v1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [Authorize]
    public class TransactionsController : ControllerBase
    {
        private readonly IMediator _mediator;
        
        public TransactionsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        /// <summary>
        /// Get paginated list of transactions
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<TransactionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResult<TransactionDto>>> GetTransactions(
            [FromQuery] GetTransactionsQuery query,
            CancellationToken cancellationToken)
        {
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
        
        /// <summary>
        /// Get transaction by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TransactionDetailDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TransactionDetailDto>> GetTransaction(
            Guid id,
            CancellationToken cancellationToken)
        {
            var query = new GetTransactionByIdQuery { Id = id };
            var result = await _mediator.Send(query, cancellationToken);
            return Ok(result);
        }
    }
}
```

---

## 6. Frontend Design

### 6.1 Application Structure

```
src/
├── api/                    # API client and configurations
│   ├── axios.config.ts
│   ├── transactions.api.ts
│   ├── disputes.api.ts
│   └── auth.api.ts
├── assets/                 # Static assets
│   ├── images/
│   └── icons/
├── components/             # Reusable components
│   ├── common/
│   │   ├── Button/
│   │   ├── Input/
│   │   ├── Modal/
│   │   ├── Table/
│   │   ├── Pagination/
│   │   ├── LoadingSpinner/
│   │   └── ErrorBoundary/
│   ├── layout/
│   │   ├── Header/
│   │   ├── Footer/
│   │   ├── Sidebar/
│   │   └── Layout.tsx
│   ├── transactions/
│   │   ├── TransactionList/
│   │   ├── TransactionCard/
│   │   ├── TransactionDetails/
│   │   └── TransactionFilters/
│   └── disputes/
│       ├── DisputeList/
│       ├── DisputeCard/
│       ├── DisputeForm/
│       ├── DisputeDetails/
│       └── DisputeHistory/
├── hooks/                  # Custom React hooks
│   ├── useAuth.ts
│   ├── useTransactions.ts
│   ├── useDisputes.ts
│   ├── usePagination.ts
│   └── useDebounce.ts
├── pages/                  # Page components
│   ├── Dashboard/
│   ├── Transactions/
│   ├── TransactionDetails/
│   ├── Disputes/
│   ├── DisputeDetails/
│   ├── CreateDispute/
│   ├── Login/
│   └── NotFound/
├── routes/                 # Routing configuration
│   ├── AppRoutes.tsx
│   └── ProtectedRoute.tsx
├── store/                  # State management
│   ├── authStore.ts
│   ├── transactionStore.ts
│   └── disputeStore.ts
├── types/                  # TypeScript types
│   ├── transaction.types.ts
│   ├── dispute.types.ts
│   └── api.types.ts
├── utils/                  # Utility functions
│   ├── formatters.ts
│   ├── validators.ts
│   ├── constants.ts
│   └── helpers.ts
├── App.tsx
├── main.tsx
└── vite.config.ts
```

### 6.2 Pages and Components

#### 6.2.1 Dashboard Page

**Purpose:** Overview of account activity and quick actions

**Components:**
- Summary cards (Total transactions, Active disputes, etc.)
- Recent transactions list
- Recent disputes list
- Quick action buttons

**Key Features:**
- Visual statistics (charts showing transaction trends)
- Alert notifications for disputes needing attention
- Quick search functionality

```tsx
// Dashboard.tsx
export const Dashboard = () => {
  const { data: summary } = useTransactionSummary();
  const { data: recentTransactions } = useRecentTransactions(5);
  const { data: recentDisputes } = useRecentDisputes(5);
  
  return (
    <Container>
      <Typography variant="h4">Dashboard</Typography>
      
      <Grid container spacing={3}>
        <Grid item xs={12} md={3}>
          <StatCard
            title="Total Transactions"
            value={summary?.totalTransactions}
            icon={<ReceiptIcon />}
          />
        </Grid>
        <Grid item xs={12} md={3}>
          <StatCard
            title="Active Disputes"
            value={summary?.pendingDisputesCount}
            icon={<DisputeIcon />}
            color="warning"
          />
        </Grid>
        {/* More stat cards */}
      </Grid>
      
      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <TransactionList
            transactions={recentTransactions}
            title="Recent Transactions"
          />
        </Grid>
        <Grid item xs={12} md={4}>
          <DisputeList
            disputes={recentDisputes}
            title="Recent Disputes"
          />
        </Grid>
      </Grid>
    </Container>
  );
};
```

#### 6.2.2 Transactions Page

**Purpose:** View and filter all transactions

**Components:**
- Search bar
- Filter panel (date range, amount, status, category)
- Sort options
- Pagination controls
- Transaction table/cards
- "Dispute Transaction" action button

**Key Features:**
- Real-time search with debouncing
- Multi-criteria filtering
- Export to CSV (optional)
- Responsive design (table on desktop, cards on mobile)

```tsx
// Transactions.tsx
export const Transactions = () => {
  const [filters, setFilters] = useState<TransactionFilters>({
    pageNumber: 1,
    pageSize: 20,
    sortBy: 'transactionDate',
    sortOrder: 'desc'
  });
  
  const { data, isLoading } = useTransactions(filters);
  
  return (
    <Container>
      <Box display="flex" justifyContent="space-between" alignItems="center">
        <Typography variant="h4">Transactions</Typography>
      </Box>
      
      <TransactionFilters
        filters={filters}
        onFiltersChange={setFilters}
      />
      
      {isLoading ? (
        <LoadingSpinner />
      ) : (
        <>
          <TransactionTable
            transactions={data?.items}
            onDisputeClick={handleDisputeClick}
          />
          <Pagination
            currentPage={data?.pageNumber}
            totalPages={data?.totalPages}
            onPageChange={handlePageChange}
          />
        </>
      )}
    </Container>
  );
};
```

#### 6.2.3 Transaction Details Page

**Purpose:** Detailed view of a single transaction

**Components:**
- Transaction information card
- "Dispute This Transaction" button
- Dispute details (if exists)
- Transaction history/timeline

#### 6.2.4 Create Dispute Page/Modal

**Purpose:** Form to create a new dispute

**Components:**
- Transaction summary (read-only)
- Dispute reason dropdown
- Description textarea with character count
- File upload for evidence (optional)
- Submit/Cancel buttons

**Validation:**
- Reason is required
- Description minimum 20 characters
- File size limits (if applicable)

```tsx
// CreateDisputeForm.tsx
export const CreateDisputeForm = ({ transaction, onSuccess, onCancel }) => {
  const { control, handleSubmit, formState: { errors } } = useForm({
    resolver: yupResolver(disputeSchema)
  });
  
  const { mutate: createDispute, isLoading } = useCreateDispute();
  
  const onSubmit = (data) => {
    createDispute(
      {
        transactionId: transaction.id,
        reason: data.reason,
        description: data.description
      },
      {
        onSuccess: () => {
          toast.success('Dispute created successfully');
          onSuccess();
        }
      }
    );
  };
  
  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <TransactionSummaryCard transaction={transaction} />
      
      <Controller
        name="reason"
        control={control}
        render={({ field }) => (
          <Select
            {...field}
            label="Dispute Reason"
            error={!!errors.reason}
            helperText={errors.reason?.message}
          >
            <MenuItem value="UnauthorizedTransaction">
              Unauthorized Transaction
            </MenuItem>
            <MenuItem value="IncorrectAmount">
              Incorrect Amount
            </MenuItem>
            {/* More options */}
          </Select>
        )}
      />
      
      <Controller
        name="description"
        control={control}
        render={({ field }) => (
          <TextField
            {...field}
            label="Description"
            multiline
            rows={4}
            error={!!errors.description}
            helperText={errors.description?.message}
            inputProps={{ maxLength: 1000 }}
          />
        )}
      />
      
      <Box display="flex" gap={2}>
        <Button type="submit" variant="contained" disabled={isLoading}>
          Submit Dispute
        </Button>
        <Button onClick={onCancel} variant="outlined">
          Cancel
        </Button>
      </Box>
    </form>
  );
};
```

#### 6.2.5 Disputes Page

**Purpose:** View all disputes with filtering

**Components:**
- Status filter tabs (All, Pending, Approved, Rejected)
- Dispute cards/table
- Search functionality
- Pagination

#### 6.2.6 Dispute Details Page

**Purpose:** Detailed view of a dispute

**Components:**
- Dispute information
- Associated transaction details
- Status timeline
- Resolution notes (if resolved)
- Cancel dispute button (if pending)
- Evidence attachments (if any)

```tsx
// DisputeDetails.tsx
export const DisputeDetails = () => {
  const { id } = useParams();
  const { data: dispute, isLoading } = useDispute(id);
  const { data: history } = useDisputeHistory(id);
  
  if (isLoading) return <LoadingSpinner />;
  
  return (
    <Container>
      <Typography variant="h4">Dispute Details</Typography>
      
      <Grid container spacing={3}>
        <Grid item xs={12} md={8}>
          <Card>
            <CardContent>
              <DisputeStatusBadge status={dispute.status} />
              <Typography variant="h6">Dispute Information</Typography>
              <Box mt={2}>
                <InfoRow label="Dispute ID" value={dispute.id} />
                <InfoRow label="Reason" value={dispute.reasonDescription} />
                <InfoRow label="Description" value={dispute.description} />
                <InfoRow label="Created" value={formatDate(dispute.createdDate)} />
                {dispute.resolvedDate && (
                  <InfoRow label="Resolved" value={formatDate(dispute.resolvedDate)} />
                )}
              </Box>
              
              {dispute.isActive && (
                <Button
                  onClick={handleCancel}
                  variant="outlined"
                  color="error"
                >
                  Cancel Dispute
                </Button>
              )}
            </CardContent>
          </Card>
          
          <Card sx={{ mt: 2 }}>
            <CardContent>
              <Typography variant="h6">Transaction Details</Typography>
              <TransactionSummaryCard transaction={dispute.transaction} />
            </CardContent>
          </Card>
        </Grid>
        
        <Grid item xs={12} md={4}>
          <Card>
            <CardContent>
              <Typography variant="h6">Timeline</Typography>
              <DisputeTimeline history={history} />
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Container>
  );
};
```

### 6.3 State Management Strategy

#### Recommended Approach: **React Query + Zustand**

**React Query (TanStack Query)** - Server State
- Manages API data fetching, caching, synchronization
- Automatic background refetching
- Optimistic updates
- Request deduplication

**Zustand** - Client State
- Lightweight state management for UI state
- Authentication state
- User preferences
- Modal/drawer states

```typescript
// store/authStore.ts
import { create } from 'zustand';
import { persist } from 'zustand/middleware';

interface AuthState {
  token: string | null;
  user: User | null;
  isAuthenticated: boolean;
  login: (token: string, user: User) => void;
  logout: () => void;
}

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      token: null,
      user: null,
      isAuthenticated: false,
      login: (token, user) => set({ token, user, isAuthenticated: true }),
      logout: () => set({ token: null, user: null, isAuthenticated: false })
    }),
    {
      name: 'auth-storage'
    }
  )
);
```

```typescript
// hooks/useTransactions.ts
import { useQuery, useMutation, useQueryClient } from '@tanstack/react-query';
import { transactionsApi } from '@/api/transactions.api';

export const useTransactions = (filters: TransactionFilters) => {
  return useQuery({
    queryKey: ['transactions', filters],
    queryFn: () => transactionsApi.getTransactions(filters),
    keepPreviousData: true,
    staleTime: 30000 // 30 seconds
  });
};

export const useCreateDispute = () => {
  const queryClient = useQueryClient();
  
  return useMutation({
    mutationFn: disputesApi.createDispute,
    onSuccess: () => {
      // Invalidate and refetch
      queryClient.invalidateQueries(['transactions']);
      queryClient.invalidateQueries(['disputes']);
    }
  });
};
```

### 6.4 UX Considerations

#### Design Principles
1. **Clarity:** Clear labels, helpful error messages
2. **Consistency:** Uniform design patterns throughout
3. **Feedback:** Loading states, success/error notifications
4. **Accessibility:** ARIA labels, keyboard navigation, color contrast
5. **Responsiveness:** Mobile-first design

#### Key UX Features
- **Loading States:** Skeleton screens, spinners
- **Error Handling:** User-friendly error messages with retry options
- **Empty States:** Helpful messages when no data exists
- **Confirmation Dialogs:** For destructive actions (cancel dispute)
- **Toast Notifications:** Non-intrusive feedback
- **Form Validation:** Real-time validation with clear error messages
- **Keyboard Shortcuts:** For power users
- **Dark Mode:** Optional theme support

#### Accessibility (WCAG 2.1 Level AA)
- Semantic HTML
- ARIA attributes
- Keyboard navigation
- Focus management
- Color contrast ratios
- Screen reader support
- Alt text for images

---

## 7. Business Logic

### 7.1 Transaction Rules

#### Transaction Creation
1. **Valid Amount:** Must be greater than 0, max 2 decimal places
2. **Valid Date:** Cannot be in the future
3. **Customer Association:** Must belong to authenticated customer
4. **Currency:** Must be valid ISO 4217 code

#### Transaction Status Transitions
```
Pending → Completed → [Disputed → Reversed]
Pending → Failed
```

### 7.2 Dispute Rules

#### Dispute Creation Rules

1. **Eligibility Check:**
   - Transaction must exist and belong to customer
   - Transaction must not already have an active dispute
   - Transaction must be in "Completed" status
   - Transaction must be within dispute window (90 days)
   - Transaction amount must be within disputable range

2. **Validation:**
   - Reason must be valid enum value
   - Description must be between 20-1000 characters
   - Evidence files (if any) must be valid formats and sizes

#### Dispute Status Transitions

```
                    ┌──────────────────┐
                    │     Pending      │
                    └────────┬─────────┘
                             │
                 ┌───────────┼───────────┐
                 │           │           │
                 ▼           ▼           ▼
          ┌──────────┐ ┌──────────┐ ┌──────────┐
          │ Cancelled│ │  Under   │ │ Rejected │
          │          │ │  Review  │ │          │
          └──────────┘ └────┬─────┘ └──────────┘
                            │
                            ▼
                      ┌──────────┐
                      │ Approved │
                      └──────────┘
```

**State Descriptions:**

- **Pending:** Initial state when dispute is created
- **Under Review:** Dispute is being investigated (optional intermediate state)
- **Approved:** Dispute is valid, refund/reversal initiated
- **Rejected:** Dispute is invalid or insufficient evidence
- **Cancelled:** Customer cancelled the dispute

**Transition Rules:**

```csharp
// Pseudo-code for validation
public bool CanTransitionTo(DisputeStatus newStatus)
{
    return (Status, newStatus) switch
    {
        (Pending, UnderReview) => true,
        (Pending, Cancelled) => true,
        (Pending, Rejected) => true,
        (UnderReview, Approved) => true,
        (UnderReview, Rejected) => true,
        (UnderReview, Cancelled) => true,
        _ => false
    };
}
```

### 7.3 Business Constraints

#### Time Constraints
- **Dispute Window:** 90 days from transaction date
- **Cancellation Window:** Only pending or under-review disputes
- **Auto-Close:** Disputes inactive for 30 days (optional)

#### Amount Constraints
- **Minimum Dispute Amount:** $1.00
- **Maximum Dispute Amount:** $10,000 (configurable)

#### Rate Limiting
- **Max Disputes per Day:** 5 per customer
- **Max Disputes per Transaction:** 1 (no re-disputes)

### 7.4 Domain Services

#### DisputeEligibilityService
```csharp
public class DisputeEligibilityService
{
    private const int MaxDisputeWindowDays = 90;
    
    public DisputeEligibilityResult CheckEligibility(Transaction transaction)
    {
        var errors = new List<string>();
        
        if (transaction.Status != TransactionStatus.Completed)
            errors.Add("Only completed transactions can be disputed");
        
        if (transaction.IsDisputed)
            errors.Add("Transaction already has an active dispute");
        
        var daysSinceTransaction = (DateTime.UtcNow - transaction.TransactionDate).Days;
        if (daysSinceTransaction > MaxDisputeWindowDays)
            errors.Add($"Dispute window has expired. Transactions can only be disputed within {MaxDisputeWindowDays} days");
        
        if (transaction.Amount <= 0)
            errors.Add("Transaction amount must be greater than zero");
        
        return new DisputeEligibilityResult
        {
            IsEligible = !errors.Any(),
            Errors = errors
        };
    }
}
```

### 7.5 Validation Rules

#### FluentValidation Examples

```csharp
public class CreateDisputeCommandValidator : AbstractValidator<CreateDisputeCommand>
{
    public CreateDisputeCommandValidator(ITransactionRepository transactionRepository)
    {
        RuleFor(x => x.TransactionId)
            .NotEmpty()
            .MustAsync(async (id, cancellation) =>
            {
                var transaction = await transactionRepository.FindByIdAsync(id, cancellation);
                return transaction != null;
            })
            .WithMessage("Transaction does not exist");
        
        RuleFor(x => x.Reason)
            .IsInEnum()
            .WithMessage("Invalid dispute reason");
        
        RuleFor(x => x.Description)
            .NotEmpty()
            .MinimumLength(20)
            .WithMessage("Description must be at least 20 characters")
            .MaximumLength(1000)
            .WithMessage("Description cannot exceed 1000 characters");
    }
}
```

---

## 8. Security Considerations

### 8.1 Authentication

#### Approach: **JWT (JSON Web Tokens)**

**Implementation:**
- Access tokens (short-lived, 15-60 minutes)
- Refresh tokens (long-lived, 7-30 days)
- Secure token storage (HttpOnly cookies or secure storage)

```csharp
// appsettings.json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-here-minimum-32-characters",
    "Issuer": "TransactionsDisputePortal",
    "Audience": "TransactionsDisputePortalUsers",
    "AccessTokenExpirationMinutes": 60,
    "RefreshTokenExpirationDays": 7
  }
}
```

```csharp
// Program.cs
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]))
        };
    });
```

**Frontend Token Management:**
```typescript
// api/axios.config.ts
const axiosInstance = axios.create({
  baseURL: import.meta.env.VITE_API_URL,
  timeout: 30000
});

// Request interceptor
axiosInstance.interceptors.request.use(
  (config) => {
    const token = useAuthStore.getState().token;
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);

// Response interceptor for token refresh
axiosInstance.interceptors.response.use(
  (response) => response,
  async (error) => {
    const originalRequest = error.config;
    
    if (error.response?.status === 401 && !originalRequest._retry) {
      originalRequest._retry = true;
      
      try {
        const { token } = await authApi.refreshToken();
        useAuthStore.getState().setToken(token);
        originalRequest.headers.Authorization = `Bearer ${token}`;
        return axiosInstance(originalRequest);
      } catch (refreshError) {
        useAuthStore.getState().logout();
        window.location.href = '/login';
      }
    }
    
    return Promise.reject(error);
  }
);
```

### 8.2 Authorization

#### Resource-Based Authorization

**Principle:** Users can only access their own data

```csharp
// Application/Common/Security/AuthorizeAttribute.cs
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class AuthorizeAttribute : Attribute
{
    public string Roles { get; set; } = string.Empty;
    public string Policy { get; set; } = string.Empty;
}

// Example Command with Authorization
[Authorize]
public class GetTransactionsQuery : IRequest<PagedResult<TransactionDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

// Authorization Behavior
public class AuthorizationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICurrentUserService _currentUserService;
    
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var authorizeAttributes = request.GetType().GetCustomAttributes<AuthorizeAttribute>();
        
        if (authorizeAttributes.Any())
        {
            // Check if user is authenticated
            if (_currentUserService.UserId == null)
            {
                throw new UnauthorizedException();
            }
            
            // Additional role/policy checks...
        }
        
        return await next();
    }
}
```

#### CurrentUserService
```csharp
public interface ICurrentUserService
{
    Guid? UserId { get; }
    string? Email { get; }
    bool IsAuthenticated { get; }
}

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }
    
    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User
                ?.FindFirstValue(ClaimTypes.NameIdentifier);
            
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }
    }
    
    public string? Email => _httpContextAccessor.HttpContext?.User
        ?.FindFirstValue(ClaimTypes.Email);
    
    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
}
```

### 8.3 Data Protection

#### Sensitive Data Handling
- **Password Hashing:** Use ASP.NET Core Identity with PBKDF2
- **Connection Strings:** Store in User Secrets (dev) or Azure Key Vault (prod)
- **API Keys:** Never commit to source control
- **PII Protection:** Encrypt sensitive fields in database (if required)

#### HTTPS Enforcement
```csharp
// Program.cs
app.UseHttpsRedirection();

// For production
builder.Services.AddHsts(options =>
{
    options.MaxAge = TimeSpan.FromDays(365);
    options.IncludeSubDomains = true;
});
```

### 8.4 Input Validation

#### Multiple Layers of Validation

1. **Client-Side:** React Hook Form + Yup/Zod
2. **API Model Validation:** Data Annotations
3. **Application Layer:** FluentValidation
4. **Domain Layer:** Domain rules and invariants

#### SQL Injection Prevention
- **Entity Framework Core:** Parameterized queries by default
- **Avoid raw SQL:** Use LINQ queries
- **If raw SQL needed:** Use parameterized queries

```csharp
// Safe query
var transactions = await _context.Transactions
    .Where(t => t.CustomerId == customerId)
    .ToListAsync();

// Safe raw SQL (if needed)
var transactions = await _context.Transactions
    .FromSqlRaw("SELECT * FROM Transactions WHERE CustomerId = {0}", customerId)
    .ToListAsync();
```

### 8.5 Cross-Origin Resource Sharing (CORS)

```csharp
// Program.cs
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(builder.Configuration["FrontendUrl"])
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

app.UseCors("AllowFrontend");
```

### 8.6 Rate Limiting

```csharp
// Using AspNetCoreRateLimit
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// appsettings.json
{
  "IpRateLimiting": {
    "EnableEndpointRateLimiting": true,
    "StackBlockedRequests": false,
    "RealIpHeader": "X-Real-IP",
    "ClientIdHeader": "X-ClientId",
    "HttpStatusCode": 429,
    "GeneralRules": [
      {
        "Endpoint": "*",
        "Period": "1m",
        "Limit": 60
      },
      {
        "Endpoint": "*/disputes",
        "Period": "1d",
        "Limit": 5
      }
    ]
  }
}
```

### 8.7 Security Headers

```csharp
app.Use(async (context, next) =>
{
    context.Response.Headers.Add("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Add("X-Frame-Options", "DENY");
    context.Response.Headers.Add("X-XSS-Protection", "1; mode=block");
    context.Response.Headers.Add("Referrer-Policy", "no-referrer");
    context.Response.Headers.Add("Content-Security-Policy", 
        "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'");
    
    await next();
});
```

---

## 9. Performance & Scalability

### 9.1 Caching Strategy

#### Multi-Level Caching

**1. In-Memory Caching (IMemoryCache)**
- Cache reference data (enums, lookups)
- Short-term cache for frequently accessed data

```csharp
public class CachedTransactionRepository : ITransactionRepository
{
    private readonly TransactionRepository _innerRepository;
    private readonly IMemoryCache _cache;
    
    public async Task<Transaction> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        var cacheKey = $"transaction:{id}";
        
        if (_cache.TryGetValue(cacheKey, out Transaction transaction))
        {
            return transaction;
        }
        
        transaction = await _innerRepository.GetByIdAsync(id, cancellationToken);
        
        _cache.Set(cacheKey, transaction, TimeSpan.FromMinutes(5));
        
        return transaction;
    }
}
```

**2. Distributed Caching (Redis)** - Optional Enhancement
- Share cache across multiple instances
- Session management
- Response caching

```csharp
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "TransactionsDisputePortal:";
});
```

**3. Response Caching**
- Cache GET responses at API level
- Use cache-control headers

```csharp
[HttpGet]
[ResponseCache(Duration = 60, Location = ResponseCacheLocation.Client)]
public async Task<ActionResult<TransactionSummaryDto>> GetSummary()
{
    // ...
}
```

**Frontend Caching:**
```typescript
// React Query caching configuration
const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 5 * 60 * 1000, // 5 minutes
      cacheTime: 10 * 60 * 1000, // 10 minutes
      refetchOnWindowFocus: false,
      retry: 1
    }
  }
});
```

### 9.2 Database Optimization

#### Indexing Strategy
```sql
-- High-impact indexes (already covered in section 4.5)
CREATE INDEX IX_Transaction_CustomerId_Date 
ON Transactions(CustomerId, TransactionDate DESC)
INCLUDE (Amount, MerchantName, Status);

CREATE INDEX IX_Dispute_CustomerId_Status 
ON Disputes(CustomerId, Status)
INCLUDE (CreatedDate, TransactionId);
```

#### Query Optimization

**1. Use AsNoTracking for Read-Only Queries**
```csharp
public async Task<List<TransactionDto>> GetTransactionsAsync(Guid customerId)
{
    return await _context.Transactions
        .AsNoTracking()
        .Where(t => t.CustomerId == customerId)
        .ProjectTo<TransactionDto>(_mapper.ConfigurationProvider)
        .ToListAsync();
}
```

**2. Projection (Select only needed columns)**
```csharp
// Good - Project to DTO
var transactions = await _context.Transactions
    .Where(t => t.CustomerId == customerId)
    .Select(t => new TransactionDto
    {
        Id = t.Id,
        Amount = t.Amount,
        MerchantName = t.MerchantName
    })
    .ToListAsync();

// Bad - Loads entire entity
var transactions = await _context.Transactions
    .Where(t => t.CustomerId == customerId)
    .ToListAsync();
```

**3. Avoid N+1 Queries**
```csharp
// Good - Use Include for related data
var disputes = await _context.Disputes
    .Include(d => d.Transaction)
    .Include(d => d.History)
    .Where(d => d.CustomerId == customerId)
    .ToListAsync();

// Bad - Causes N+1 queries
var disputes = await _context.Disputes
    .Where(d => d.CustomerId == customerId)
    .ToListAsync();
// Then accessing dispute.Transaction for each item
```

**4. Compiled Queries** (Optional for hot paths)
```csharp
private static readonly Func<ApplicationDbContext, Guid, Task<Transaction>> GetTransactionByIdQuery =
    EF.CompileAsyncQuery((ApplicationDbContext context, Guid id) =>
        context.Transactions.FirstOrDefault(t => t.Id == id));

public async Task<Transaction> GetByIdAsync(Guid id)
{
    return await GetTransactionByIdQuery(_context, id);
}
```

### 9.3 Pagination Strategy

#### Server-Side Pagination
```csharp
public class PagedResult<T>
{
    public List<T> Items { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;
}

public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
    this IQueryable<T> query,
    int pageNumber,
    int pageSize,
    CancellationToken cancellationToken)
{
    var count = await query.CountAsync(cancellationToken);
    var items = await query
        .Skip((pageNumber - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);
    
    return new PagedResult<T>
    {
        Items = items,
        PageNumber = pageNumber,
        PageSize = pageSize,
        TotalCount = count,
        TotalPages = (int)Math.Ceiling(count / (double)pageSize)
    };
}
```

#### Frontend Infinite Scroll (Optional)
```typescript
export const useInfiniteTransactions = (filters: TransactionFilters) => {
  return useInfiniteQuery({
    queryKey: ['transactions', 'infinite', filters],
    queryFn: ({ pageParam = 1 }) =>
      transactionsApi.getTransactions({ ...filters, pageNumber: pageParam }),
    getNextPageParam: (lastPage) =>
      lastPage.hasNextPage ? lastPage.pageNumber + 1 : undefined
  });
};
```

### 9.4 Asynchronous Processing

#### Background Jobs (Optional Enhancement)

**Use Case:** Send email notifications for dispute updates

```csharp
// Using Hangfire
public class DisputeNotificationJob
{
    private readonly IEmailService _emailService;
    
    public async Task SendDisputeCreatedEmail(Guid disputeId)
    {
        // Load dispute details
        // Send email
    }
}

// In dispute creation handler
BackgroundJob.Enqueue<DisputeNotificationJob>(job => 
    job.SendDisputeCreatedEmail(dispute.Id));
```

### 9.5 API Performance

#### Compression
```csharp
builder.Services.AddResponseCompression(options =>
{
    options.EnableForHttps = true;
    options.Providers.Add<GzipCompressionProvider>();
    options.Providers.Add<BrotliCompressionProvider>();
});
```

#### Output Caching (ASP.NET Core 7+)
```csharp
builder.Services.AddOutputCache(options =>
{
    options.AddBasePolicy(builder => builder.Expire(TimeSpan.FromSeconds(60)));
});

app.UseOutputCache();

// In controller
[HttpGet]
[OutputCache(Duration = 60)]
public async Task<ActionResult<TransactionSummaryDto>> GetSummary()
{
    // ...
}
```

### 9.6 Scalability Considerations

#### Horizontal Scaling
- **Stateless API:** No in-process state, scale out easily
- **Load Balancing:** Use Azure App Service, AWS ELB, or Kubernetes
- **Database Scaling:** Read replicas for read-heavy workloads

#### Vertical Scaling
- Increase CPU/RAM for API and database instances
- Monitor and scale based on metrics

#### Database Partitioning (Future)
- Partition by CustomerId for multi-tenant scenarios
- Archive old transactions to separate tables/databases

#### Monitoring
```csharp
// Application Insights
builder.Services.AddApplicationInsightsTelemetry();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>()
    .AddUrlGroup(new Uri("https://external-service.com/health"), "External Service");
```

---

## 10. Testing Strategy

### 10.1 Unit Testing

#### Test Pyramid
```
        /\
       /  \
      / UI \          E2E Tests (Few)
     /______\
    /        \
   / Integr.  \      Integration Tests (Some)
  /____________\
 /              \
/     Unit       \   Unit Tests (Many)
/__________________\
```

#### Backend Unit Tests

**Tools:**
- xUnit
- FluentAssertions
- Moq

**Test Structure:**
```
TransactionsDisputePortal.Tests/
├── Domain.Tests/
│   ├── Entities/
│   │   ├── TransactionTests.cs
│   │   ├── DisputeTests.cs
│   │   └── CustomerTests.cs
│   └── ValueObjects/
│       └── MoneyTests.cs
├── Application.Tests/
│   ├── Commands/
│   │   ├── CreateDisputeCommandHandlerTests.cs
│   │   └── CancelDisputeCommandHandlerTests.cs
│   ├── Queries/
│   │   ├── GetTransactionsQueryHandlerTests.cs
│   │   └── GetDisputeByIdQueryHandlerTests.cs
│   └── Validators/
│       └── CreateDisputeCommandValidatorTests.cs
└── Api.Tests/
    └── Controllers/
        ├── TransactionsControllerTests.cs
        └── DisputesControllerTests.cs
```

**Example Unit Tests:**

```csharp
// Domain.Tests/Entities/DisputeTests.cs
public class DisputeTests
{
    [Fact]
    public void Create_ValidInputs_ReturnsDisputeWithPendingStatus()
    {
        // Arrange
        var transactionId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var reason = DisputeReason.UnauthorizedTransaction;
        var description = "This transaction was not authorized by me.";
        
        // Act
        var dispute = Dispute.Create(transactionId, customerId, reason, description);
        
        // Assert
        dispute.Should().NotBeNull();
        dispute.Id.Should().NotBeEmpty();
        dispute.Status.Should().Be(DisputeStatus.Pending);
        dispute.IsActive.Should().BeTrue();
        dispute.History.Should().HaveCount(1);
    }
    
    [Fact]
    public void Approve_PendingDispute_ChangesStatusToApproved()
    {
        // Arrange
        var dispute = CreateTestDispute();
        var notes = "Refund approved";
        var approvedBy = "admin@example.com";
        
        // Act
        dispute.Approve(notes, approvedBy);
        
        // Assert
        dispute.Status.Should().Be(DisputeStatus.Approved);
        dispute.IsActive.Should().BeFalse();
        dispute.ResolvedDate.Should().NotBeNull();
        dispute.ResolutionNotes.Should().Be(notes);
        dispute.History.Should().HaveCount(2);
    }
    
    [Fact]
    public void Approve_AlreadyResolvedDispute_ThrowsException()
    {
        // Arrange
        var dispute = CreateTestDispute();
        dispute.Approve("First approval", "admin@example.com");
        
        // Act & Assert
        var act = () => dispute.Approve("Second approval", "admin@example.com");
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Only active disputes can be approved");
    }
    
    private Dispute CreateTestDispute()
    {
        return Dispute.Create(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DisputeReason.UnauthorizedTransaction,
            "Test dispute description with sufficient length");
    }
}
```

```csharp
// Application.Tests/Commands/CreateDisputeCommandHandlerTests.cs
public class CreateDisputeCommandHandlerTests
{
    private readonly Mock<ITransactionRepository> _transactionRepositoryMock;
    private readonly Mock<IDisputeRepository> _disputeRepositoryMock;
    private readonly Mock<ICurrentUserService> _currentUserServiceMock;
    private readonly CreateDisputeCommandHandler _handler;
    
    public CreateDisputeCommandHandlerTests()
    {
        _transactionRepositoryMock = new Mock<ITransactionRepository>();
        _disputeRepositoryMock = new Mock<IDisputeRepository>();
        _currentUserServiceMock = new Mock<ICurrentUserService>();
        
        _handler = new CreateDisputeCommandHandler(
            _transactionRepositoryMock.Object,
            _disputeRepositoryMock.Object,
            _currentUserServiceMock.Object);
    }
    
    [Fact]
    public async Task Handle_ValidCommand_CreatesDispute()
    {
        // Arrange
        var customerId = Guid.NewGuid();
        var transaction = Transaction.Create(
            customerId,
            DateTime.UtcNow.AddDays(-1),
            100.00m,
            "USD",
            "Test Merchant",
            "Test transaction",
            "Shopping",
            TransactionType.Debit);
        
        _currentUserServiceMock.Setup(x => x.UserId).Returns(customerId);
        _transactionRepositoryMock
            .Setup(x => x.FindByIdAsync(transaction.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(transaction);
        
        var command = new CreateDisputeCommand
        {
            TransactionId = transaction.Id,
            Reason = DisputeReason.UnauthorizedTransaction,
            Description = "This is a test dispute description with sufficient length"
        };
        
        // Act
        var result = await _handler.Handle(command, CancellationToken.None);
        
        // Assert
        result.Should().NotBeNull();
        result.DisputeId.Should().NotBeEmpty();
        
        _disputeRepositoryMock.Verify(
            x => x.AddAsync(It.Is<Dispute>(d =>
                d.TransactionId == transaction.Id &&
                d.Status == DisputeStatus.Pending),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
    
    [Fact]
    public async Task Handle_TransactionNotFound_ThrowsNotFoundException()
    {
        // Arrange
        _currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.NewGuid());
        _transactionRepositoryMock
            .Setup(x => x.FindByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Transaction)null);
        
        var command = new CreateDisputeCommand
        {
            TransactionId = Guid.NewGuid(),
            Reason = DisputeReason.UnauthorizedTransaction,
            Description = "This is a test dispute description"
        };
        
        // Act & Assert
        await _handler
            .Invoking(h => h.Handle(command, CancellationToken.None))
            .Should().ThrowAsync<NotFoundException>();
    }
}
```

### 10.2 Integration Testing

**Tools:**
- WebApplicationFactory
- Testcontainers (for database)
- Respawn (database cleanup)

```csharp
// Api.IntegrationTests/Controllers/DisputesControllerTests.cs
public class DisputesControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<Program> _factory;
    
    public DisputesControllerTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Replace database with in-memory or test container
                services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });
        
        _client = _factory.CreateClient();
    }
    
    [Fact]
    public async Task CreateDispute_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var token = await GetAuthTokenAsync();
        _client.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", token);
        
        var transaction = await CreateTestTransactionAsync();
        
        var request = new
        {
            transactionId = transaction.Id,
            reason = "UnauthorizedTransaction",
            description = "This is a test dispute description with sufficient length"
        };
        
        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/disputes", request);
        
        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<DisputeDto>();
        result.Should().NotBeNull();
        result.Status.Should().Be("Pending");
    }
}
```

### 10.3 Frontend Testing

#### Unit Tests (Components)

**Tools:**
- Vitest
- React Testing Library
- Mock Service Worker (MSW)

```typescript
// components/disputes/DisputeForm.test.tsx
import { render, screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { DisputeForm } from './DisputeForm';

describe('DisputeForm', () => {
  const mockTransaction = {
    id: '123',
    amount: 100.00,
    merchantName: 'Test Merchant'
  };
  
  it('renders form with transaction details', () => {
    render(<DisputeForm transaction={mockTransaction} />);
    
    expect(screen.getByText('Test Merchant')).toBeInTheDocument();
    expect(screen.getByText('$100.00')).toBeInTheDocument();
  });
  
  it('shows validation error when description is too short', async () => {
    const user = userEvent.setup();
    render(<DisputeForm transaction={mockTransaction} />);
    
    const descriptionInput = screen.getByLabelText('Description');
    await user.type(descriptionInput, 'Short');
    await user.click(screen.getByRole('button', { name: 'Submit' }));
    
    await waitFor(() => {
      expect(screen.getByText(/at least 20 characters/i)).toBeInTheDocument();
    });
  });
  
  it('calls onSubmit with form data when valid', async () => {
    const mockOnSubmit = vi.fn();
    const user = userEvent.setup();
    
    render(
      <DisputeForm
        transaction={mockTransaction}
        onSubmit={mockOnSubmit}
      />
    );
    
    await user.selectOptions(
      screen.getByLabelText('Reason'),
      'UnauthorizedTransaction'
    );
    await user.type(
      screen.getByLabelText('Description'),
      'This is a valid dispute description with more than 20 characters'
    );
    await user.click(screen.getByRole('button', { name: 'Submit' }));
    
    await waitFor(() => {
      expect(mockOnSubmit).toHaveBeenCalledWith({
        transactionId: '123',
        reason: 'UnauthorizedTransaction',
        description: expect.any(String)
      });
    });
  });
});
```

#### E2E Tests (Optional)

**Tools:**
- Playwright or Cypress

```typescript
// e2e/dispute-creation.spec.ts
import { test, expect } from '@playwright/test';

test.describe('Dispute Creation Flow', () => {
  test.beforeEach(async ({ page }) => {
    // Login
    await page.goto('/login');
    await page.fill('input[name="email"]', 'test@example.com');
    await page.fill('input[name="password"]', 'password');
    await page.click('button[type="submit"]');
    
    await expect(page).toHaveURL('/dashboard');
  });
  
  test('user can create a dispute for a transaction', async ({ page }) => {
    // Navigate to transactions
    await page.click('a[href="/transactions"]');
    
    // Select a transaction
    await page.click('button:has-text("Dispute"):first');
    
    // Fill dispute form
    await page.selectOption('select[name="reason"]', 'UnauthorizedTransaction');
    await page.fill(
      'textarea[name="description"]',
      'I did not authorize this transaction. My card was stolen.'
    );
    
    // Submit
    await page.click('button:has-text("Submit Dispute")');
    
    // Verify success
    await expect(page.locator('text=Dispute created successfully')).toBeVisible();
    await expect(page).toHaveURL(/\/disputes\/[a-f0-9-]+/);
  });
});
```

### 10.4 Test Coverage Goals

- **Unit Tests:** 80%+ code coverage
- **Integration Tests:** Critical paths (auth, dispute creation, transaction queries)
- **E2E Tests:** Happy paths only (login, create dispute, view disputes)

---

## 11. Deployment Approach

### 11.1 Local Development Setup

#### Prerequisites
- .NET 8.0 SDK
- Node.js 18+ and npm/yarn
- SQL Server LocalDB or PostgreSQL
- Visual Studio 2022 / Rider / VS Code
- Git

#### Backend Setup

```bash
# Clone repository
git clone https://github.com/yourusername/transactions-dispute-portal.git
cd transactions-dispute-portal/TransactionsDisputePortal

# Restore NuGet packages
dotnet restore

# Update database connection string in appsettings.Development.json
# Run migrations
dotnet ef database update --project TransactionsDisputePortal.Infrastructure

# Run API
cd TransactionsDisputePortal.Api
dotnet run
```

#### Frontend Setup

```bash
cd transactions-dispute-portal-ui

# Install dependencies
npm install

# Create .env.local file
echo "VITE_API_URL=https://localhost:7000" > .env.local

# Run development server
npm run dev
```

#### Database Seeding (Optional)

```csharp
// Infrastructure/Persistence/ApplicationDbContextSeed.cs
public static class ApplicationDbContextSeed
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (!context.Customers.Any())
        {
            var customer = Customer.Create(
                "demo@example.com",
                "Demo",
                "User");
            
            context.Customers.Add(customer);
            
            // Add sample transactions
            var transactions = new[]
            {
                Transaction.Create(
                    customer.Id,
                    DateTime.UtcNow.AddDays(-5),
                    125.50m,
                    "USD",
                    "Amazon.com",
                    "Electronics purchase",
                    "Shopping",
                    TransactionType.Debit),
                // More transactions...
            };
            
            context.Transactions.AddRange(transactions);
            await context.SaveChangesAsync();
        }
    }
}
```

### 11.2 Docker Setup

#### Dockerfile (Backend)

```dockerfile
# TransactionsDisputePortal.Api/Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["TransactionsDisputePortal.Api/TransactionsDisputePortal.Api.csproj", "TransactionsDisputePortal.Api/"]
COPY ["TransactionsDisputePortal.Application/TransactionsDisputePortal.Application.csproj", "TransactionsDisputePortal.Application/"]
COPY ["TransactionsDisputePortal.Domain/TransactionsDisputePortal.Domain.csproj", "TransactionsDisputePortal.Domain/"]
COPY ["TransactionsDisputePortal.Infrastructure/TransactionsDisputePortal.Infrastructure.csproj", "TransactionsDisputePortal.Infrastructure/"]

RUN dotnet restore "TransactionsDisputePortal.Api/TransactionsDisputePortal.Api.csproj"
COPY . .
WORKDIR "/src/TransactionsDisputePortal.Api"
RUN dotnet build "TransactionsDisputePortal.Api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "TransactionsDisputePortal.Api.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "TransactionsDisputePortal.Api.dll"]
```

#### Dockerfile (Frontend)

```dockerfile
# Dockerfile
FROM node:18-alpine AS build
WORKDIR /app
COPY package*.json ./
RUN npm ci
COPY . .
RUN npm run build

FROM nginx:alpine
COPY --from=build /app/dist /usr/share/nginx/html
COPY nginx.conf /etc/nginx/nginx.conf
EXPOSE 80
CMD ["nginx", "-g", "daemon off;"]
```

#### Docker Compose

```yaml
# docker-compose.yml
version: '3.8'

services:
  api:
    build:
      context: ./TransactionsDisputePortal
      dockerfile: TransactionsDisputePortal.Api/Dockerfile
    ports:
      - "7000:80"
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ConnectionStrings__DefaultConnection=Server=db;Database=TransactionsDisputePortalDb;User=sa;Password=YourStrong!Passw0rd;TrustServerCertificate=True
      - JwtSettings__SecretKey=your-secret-key-here-minimum-32-characters
    depends_on:
      - db
  
  frontend:
    build:
      context: ./transactions-dispute-portal-ui
      dockerfile: Dockerfile
    ports:
      - "3000:80"
    environment:
      - VITE_API_URL=http://localhost:7000
    depends_on:
      - api
  
  db:
    image: mcr.microsoft.com/mssql/server:2022-latest
    ports:
      - "1433:1433"
    environment:
      - ACCEPT_EULA=Y
      - SA_PASSWORD=YourStrong!Passw0rd
    volumes:
      - sqldata:/var/opt/mssql

volumes:
  sqldata:
```

### 11.3 Azure Deployment

#### Resources Required
1. **Azure App Service** (API)
2. **Azure Static Web Apps** (Frontend)
3. **Azure SQL Database** (Database)
4. **Azure Key Vault** (Secrets)
5. **Application Insights** (Monitoring)

#### Deployment Steps

**1. Create Azure Resources**

```bash
# Azure CLI
az login

# Create resource group
az group create --name rg-transactions-dispute-portal --location eastus

# Create App Service Plan
az appservice plan create \
  --name asp-transactions-dispute \
  --resource-group rg-transactions-dispute-portal \
  --sku B1 --is-linux

# Create Web App
az webapp create \
  --name api-transactions-dispute \
  --resource-group rg-transactions-dispute-portal \
  --plan asp-transactions-dispute \
  --runtime "DOTNET|8.0"

# Create Azure SQL Database
az sql server create \
  --name sql-transactions-dispute \
  --resource-group rg-transactions-dispute-portal \
  --location eastus \
  --admin-user sqladmin \
  --admin-password YourStrongPassword123!

az sql db create \
  --name TransactionsDisputePortalDb \
  --server sql-transactions-dispute \
  --resource-group rg-transactions-dispute-portal \
  --service-objective S0

# Create Static Web App
az staticwebapp create \
  --name swa-transactions-dispute \
  --resource-group rg-transactions-dispute-portal \
  --location eastus2
```

**2. GitHub Actions CI/CD**

```yaml
# .github/workflows/deploy-api.yml
name: Deploy API to Azure

on:
  push:
    branches: [ main ]
    paths:
      - 'TransactionsDisputePortal/**'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Restore dependencies
      run: dotnet restore TransactionsDisputePortal/TransactionsDisputePortal.sln
    
    - name: Build
      run: dotnet build TransactionsDisputePortal/TransactionsDisputePortal.sln --configuration Release --no-restore
    
    - name: Test
      run: dotnet test TransactionsDisputePortal/TransactionsDisputePortal.sln --no-build --verbosity normal
    
    - name: Publish
      run: dotnet publish TransactionsDisputePortal/TransactionsDisputePortal.Api/TransactionsDisputePortal.Api.csproj -c Release -o ./publish
    
    - name: Deploy to Azure Web App
      uses: azure/webapps-deploy@v2
      with:
        app-name: api-transactions-dispute
        publish-profile: ${{ secrets.AZURE_WEBAPP_PUBLISH_PROFILE }}
        package: ./publish
```

```yaml
# .github/workflows/deploy-frontend.yml
name: Deploy Frontend to Azure Static Web Apps

on:
  push:
    branches: [ main ]
    paths:
      - 'transactions-dispute-portal-ui/**'

jobs:
  build-and-deploy:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Setup Node.js
      uses: actions/setup-node@v3
      with:
        node-version: '18'
    
    - name: Install dependencies
      working-directory: ./transactions-dispute-portal-ui
      run: npm ci
    
    - name: Run tests
      working-directory: ./transactions-dispute-portal-ui
      run: npm test
    
    - name: Build
      working-directory: ./transactions-dispute-portal-ui
      run: npm run build
      env:
        VITE_API_URL: https://api-transactions-dispute.azurewebsites.net
    
    - name: Deploy to Azure Static Web Apps
      uses: Azure/static-web-apps-deploy@v1
      with:
        azure_static_web_apps_api_token: ${{ secrets.AZURE_STATIC_WEB_APPS_API_TOKEN }}
        repo_token: ${{ secrets.GITHUB_TOKEN }}
        action: "upload"
        app_location: "/transactions-dispute-portal-ui"
        output_location: "dist"
```

### 11.4 Configuration Management

**Development:**
- `appsettings.Development.json`
- `User Secrets` for sensitive data

**Production:**
- **Azure App Service Configuration**
- **Azure Key Vault** for secrets
- **Environment Variables**

```csharp
// Program.cs - Key Vault integration
builder.Configuration.AddAzureKeyVault(
    new Uri($"https://{builder.Configuration["KeyVaultName"]}.vault.azure.net/"),
    new DefaultAzureCredential());
```

---

## 12. Optional Enhancements

### 12.1 Priority 1 (Medium Effort, High Value)

#### 1. Email Notifications
- Send email when dispute is created
- Notify customer of status changes
- Implementation: SMTP service or Azure Communication Services

#### 2. File Upload for Evidence
- Allow customers to upload supporting documents
- Store in Azure Blob Storage
- Maximum 5 files, 5MB each

#### 3. Export Functionality
- Export transactions to CSV/Excel
- Export dispute history to PDF
- Implementation: EPPlus or ClosedXML

#### 4. Advanced Filtering
- Date range picker
- Multi-select for categories
- Amount range slider
- Saved filter presets

### 12.2 Priority 2 (Higher Effort)

#### 5. Admin Portal
- Separate admin interface for dispute management
- Review and resolve disputes
- View all customer disputes
- Generate reports

#### 6. Real-Time Notifications
- SignalR for real-time updates
- Push notifications for status changes
- Live dashboard updates

#### 7. Audit Logging
- Comprehensive audit trail
- Track all data changes
- Compliance and security

#### 8. Multi-Currency Support
- Support multiple currencies
- Currency conversion
- Display amounts in user's preferred currency

#### 9. Advanced Analytics
- Dispute trends dashboard
- Transaction analysis charts
- Merchant risk scoring
- Predictive dispute detection

### 12.3 Priority 3 (Future Considerations)

#### 10. Mobile Application
- React Native or .NET MAUI
- Native iOS/Android apps
- Push notifications

#### 11. Two-Factor Authentication
- SMS or authenticator app
- Enhanced security

#### 12. Chatbot Support
- AI-powered chat for dispute assistance
- Azure Bot Service integration

#### 13. Blockchain Integration
- Immutable dispute records
- Transparent audit trail

#### 14. Machine Learning
- Fraud detection
- Anomaly detection in transactions
- Automated dispute categorization

---

## 13. Implementation Timeline

### Phase 1: Foundation (Week 1-2)
- ✅ Project structure (Already exists)
- ✅ Clean Architecture layers (Already exists)
- **TODO:** Domain entities and value objects
- **TODO:** Database migrations
- **TODO:** Repository implementations

### Phase 2: Core Features - Backend (Week 2-3)
- **TODO:** CQRS commands and queries
- **TODO:** Transaction management endpoints
- **TODO:** Dispute management endpoints
- **TODO:** Validation and error handling
- **TODO:** Unit tests for domain and application layers

### Phase 3: Core Features - Frontend (Week 3-4)
- **TODO:** Project setup and routing
- **TODO:** Authentication UI
- **TODO:** Transactions list and details pages
- **TODO:** Dispute creation form
- **TODO:** Dispute management pages
- **TODO:** API integration with React Query

### Phase 4: Polish & Testing (Week 4)
- **TODO:** Integration tests
- **TODO:** Frontend component tests
- **TODO:** UI/UX improvements
- **TODO:** Performance optimization
- **TODO:** Documentation

### Phase 5: Deployment (Week 5)
- **TODO:** Docker setup
- **TODO:** CI/CD pipelines
- **TODO:** Azure deployment
- **TODO:** Smoke testing in production
- **TODO:** Final documentation and demo preparation

---

## 14. Key Architectural Decisions (ADR)

### ADR-001: Clean Architecture Pattern
**Decision:** Use Clean Architecture (Onion Architecture) with CQRS pattern

**Rationale:**
- Clear separation of concerns
- Testable business logic
- Independent of frameworks and UI
- Demonstrates senior-level architectural thinking

### ADR-002: CQRS with MediatR
**Decision:** Use CQRS pattern implemented via MediatR library

**Rationale:**
- Separates read and write operations
- Simplifies complex business logic
- Enables pipeline behaviors for cross-cutting concerns
- Improves testability

### ADR-003: Entity Framework Core
**Decision:** Use EF Core as the ORM

**Rationale:**
- Native .NET integration
- Code-first migrations
- LINQ support for type-safe queries
- Good performance with proper optimization

### ADR-004: JWT Authentication
**Decision:** Use JWT tokens for authentication

**Rationale:**
- Stateless authentication
- Scalable across multiple servers
- Industry standard
- Easy integration with frontend

### ADR-005: React with TypeScript
**Decision:** Use React with TypeScript for frontend

**Rationale:**
- Popular and well-supported
- Type safety with TypeScript
- Large ecosystem of libraries
- Good performance

### ADR-006: React Query for Server State
**Decision:** Use TanStack Query (React Query) for server state management

**Rationale:**
- Automatic caching and synchronization
- Reduces boilerplate code
- Built-in loading and error states
- Optimistic updates support

---

## 15. Success Criteria

### Functional Requirements ✓
- ✅ Customers can view their transactions
- ✅ Customers can create disputes for transactions
- ✅ Customers can view dispute status and history
- ✅ Disputes follow proper status workflow
- ✅ System validates business rules

### Non-Functional Requirements ✓
- ✅ Clean Architecture implemented correctly
- ✅ SOLID principles applied throughout
- ✅ Comprehensive error handling
- ✅ Security best practices (authentication, authorization)
- ✅ API documentation (Swagger)
- ✅ Unit test coverage > 70%
- ✅ Responsive UI design
- ✅ Performance optimizations (caching, pagination)

### Interview Assessment Goals ✓
- ✅ Demonstrates senior-level thinking
- ✅ Production-ready code quality
- ✅ Scalable architecture
- ✅ Comprehensive documentation
- ✅ Best practices and design patterns

---

## 16. References & Resources

### Documentation
- [Clean Architecture by Robert C. Martin](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
- [ASP.NET Core Documentation](https://docs.microsoft.com/en-us/aspnet/core/)
- [Entity Framework Core Documentation](https://docs.microsoft.com/en-us/ef/core/)
- [React Documentation](https://react.dev/)
- [MediatR Documentation](https://github.com/jbogard/MediatR)

### Design Patterns
- [Enterprise Application Patterns](https://martinfowler.com/eaaCatalog/)
- [CQRS Pattern](https://martinfowler.com/bliki/CQRS.html)
- [Repository Pattern](https://martinfowler.com/eaaCatalog/repository.html)

### Security
- [OWASP Top 10](https://owasp.org/www-project-top-ten/)
- [JWT Best Practices](https://tools.ietf.org/html/rfc8725)

---

## Appendix A: Glossary

| Term | Definition |
|------|------------|
| **CQRS** | Command Query Responsibility Segregation - pattern separating read and write operations |
| **MediatR** | Library implementing the Mediator pattern for decoupling requests from handlers |
| **Clean Architecture** | Architecture pattern with dependency inversion, keeping business logic independent |
| **Value Object** | Immutable object defined by its attributes rather than identity |
| **Aggregate Root** | Entity that controls access to other entities in a consistency boundary |
| **DTO** | Data Transfer Object - object used to transfer data between layers |
| **Unit of Work** | Pattern that maintains a list of objects affected by a business transaction |

---

## Appendix B: Environment Variables

### Backend (.NET)
```
ASPNETCORE_ENVIRONMENT=Development
ConnectionStrings__DefaultConnection=...
JwtSettings__SecretKey=...
JwtSettings__Issuer=...
JwtSettings__Audience=...
JwtSettings__AccessTokenExpirationMinutes=60
```

### Frontend (React)
```
VITE_API_URL=https://localhost:7000
VITE_APP_NAME=Transactions Dispute Portal
VITE_ENABLE_LOGGING=true
```

---

**Document Version:** 1.0  
**Last Updated:** May 4, 2026  
**Status:** Ready for Implementation
