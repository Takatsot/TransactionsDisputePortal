# Transactions Dispute Portal

A comprehensive financial transaction dispute management system built with **Clean Architecture**, **CQRS pattern**, and modern web technologies.

## 🏗️ Architecture Overview

This application follows **Clean Architecture** principles with clear separation of concerns:

```
┌─────────────────────────────────────────────────────────────┐
│                    Presentation Layer                        │
│  ┌──────────────────┐          ┌──────────────────────┐    │
│  │   React UI       │          │    ASP.NET Core API  │    │
│  │  (TypeScript)    │  ◄─────► │     (Controllers)    │    │
│  └──────────────────┘          └──────────────────────┘    │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                    Application Layer                         │
│  - CQRS Commands & Queries (MediatR)                        │
│  - DTOs, Mappings (AutoMapper)                              │
│  - Validation (FluentValidation)                            │
│  - Behaviours (Validation, Authorization, Logging)          │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                      Domain Layer                            │
│  - Entities (Transaction, Dispute, Customer)                │
│  - Value Objects (Money)                                     │
│  - Domain Events                                             │
│  - Business Rules & Validation                               │
│  - Repository Interfaces                                     │
└─────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌─────────────────────────────────────────────────────────────┐
│                  Infrastructure Layer                        │
│  - Entity Framework Core                                     │
│  - Repository Implementations                                │
│  - External Services (File Storage, Auth)                   │
│  - Database Migrations                                       │
└─────────────────────────────────────────────────────────────┘
```

## ✨ Key Features

### Core Functionality
- 💳 **Transaction Management** - View and track financial transactions
- ⚡ **Dispute Creation** - File disputes with supporting documents
- 📎 **File Attachments** - Upload receipts, statements (max 10MB, 5 files)
- 🔄 **Dispute Lifecycle** - Track status: Pending → Under Review → Approved/Rejected
- ❌ **Cancel Disputes** - Ability to withdraw disputes and re-dispute
- 📊 **Dashboard** - Real-time statistics and insights
- 🔍 **Advanced Filtering** - Filter by status, date, amount

### Technical Highlights
- 🏛️ **Clean Architecture** - Maintainable, testable, technology-independent
- 📨 **CQRS Pattern** - Separate read and write operations
- 🔐 **JWT Authentication** - Secure token-based auth (24h expiry)
- ✅ **FluentValidation** - Comprehensive input validation
- 🧪 **Unit Tests** - 23 passing tests for domain logic
- 🩺 **Health Checks** - Monitor database and file storage
- 📝 **Swagger/OpenAPI** - Auto-generated API documentation
- 🗄️ **Entity Framework Core** - Code-first with migrations
- 🎨 **Material-UI** - Modern, responsive React interface
- 🇿🇦 **South African Context** - Local brands (Checkers, Takealot, TFG, Woolworths)

## 🚀 Getting Started

### Prerequisites
- .NET 10.0 SDK
- Node.js 18+ and npm
- SQL Server LocalDB or SQL Server
- Visual Studio 2022 / VS Code (optional)

### Backend Setup

1. **Navigate to API project**
   ```bash
   cd TransactionsDisputePortal/TransactionsDisputePortal.Api
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Update database connection** (optional)
   Edit `appsettings.json` if not using LocalDB:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TransactionsDisputePortalDb;Trusted_Connection=true;"
   }
   ```

4. **Run migrations**
   ```bash
   cd ../TransactionsDisputePortal.Infrastructure
   dotnet ef database update --startup-project ../TransactionsDisputePortal.Api
   ```

5. **Run the API**
   ```bash
   cd ../TransactionsDisputePortal.Api
   dotnet run
   ```
   API will be available at: `https://localhost:44341`
   Swagger UI: `https://localhost:44341/swagger`
   Health Checks: `https://localhost:44341/health`

### Frontend Setup

1. **Navigate to UI project**
   ```bash
   cd TransactionsDisputePortal.UI
   ```

2. **Install dependencies**
   ```bash
   npm install
   ```

3. **Start development server**
   ```bash
   npm run dev
   ```
   UI will be available at: `http://localhost:3000`

### Test Credentials
- **Email**: `testuser@email.com`
- **Password**: `Password123!`
- **Customer**: Thabo Kapiteni

## 🧪 Running Tests

```bash
# Run all tests
cd TransactionsDisputePortal
dotnet test

# Run domain tests only
cd TransactionsDisputePortal.Domain.Tests
dotnet test --verbosity normal

# View test results
dotnet test --logger "console;verbosity=detailed"
```

**Test Coverage**: 23 passing tests covering:
- Dispute creation, cancellation, approval, rejection
- Transaction validation and state transitions
- Business rule enforcement
- Domain invariants

## 📊 Health Checks

The API exposes health check endpoints for monitoring:

| Endpoint | Purpose | Status Codes |
|----------|---------|--------------|
| `/health` | Overall system health | 200 (Healthy), 503 (Unhealthy) |
| `/health/ready` | Readiness probe (K8s) | 200 (Ready), 503 (Not Ready) |
| `/health/live` | Liveness probe (K8s) | 200 (Alive) |

Health checks monitor:
- ✅ Database connectivity (SQL Server)
- ✅ File storage accessibility

## 🏛️ Architecture Decisions

### Why Clean Architecture?
- **Independence**: Business logic isolated from frameworks and UI
- **Testability**: Domain logic testable without dependencies
- **Flexibility**: Easy to swap infrastructure components
- **Maintainability**: Clear boundaries and responsibilities

### Why CQRS?
- **Scalability**: Read and write operations scale independently
- **Performance**: Optimized queries without domain complexity
- **Clarity**: Clear distinction between commands (write) and queries (read)
- **Audit**: Easy to track all state-changing operations

### Technology Choices
| Technology | Why? |
|-----------|------|
| **ASP.NET Core 10** | Latest features, performance, cross-platform |
| **Entity Framework Core** | Mature ORM, migrations, LINQ support |
| **MediatR** | CQRS implementation, clean handler pattern |
| **FluentValidation** | Expressive, reusable validation rules |
| **AutoMapper** | Simplify DTO mappings |
| **React + TypeScript** | Type safety, modern UI development |
| **Material-UI** | Professional, accessible components |
| **TanStack Query** | Powerful data fetching and caching |
| **xUnit + FluentAssertions** | Clean, readable tests |

## 📁 Project Structure

```
TransactionsDisputePortal/
├── TransactionsDisputePortal.Api/          # Web API (Presentation)
│   ├── Controllers/                        # API endpoints
│   ├── Configuration/                      # App configuration
│   │   ├── SwashbuckleConfiguration.cs
│   │   ├── HealthChecksConfiguration.cs
│   │   └── ApplicationSecurityConfiguration.cs
│   └── HealthChecks/                       # Health check implementations
├── TransactionsDisputePortal.Application/  # Application Layer
│   ├── Common/                             # Shared logic
│   │   ├── Behaviours/                     # MediatR pipeline behaviours
│   │   ├── Interfaces/                     # Service interfaces
│   │   └── Models/                         # DTOs
│   ├── Disputes/                           # Dispute feature
│   │   ├── Commands/                       # Write operations
│   │   │   ├── CreateDispute/
│   │   │   └── CancelDispute/
│   │   ├── Queries/                        # Read operations
│   │   │   ├── GetDisputes/
│   │   │   └── GetDisputeById/
│   │   └── Validators/                     # FluentValidation rules
│   └── Transactions/                       # Transaction feature
├── TransactionsDisputePortal.Domain/       # Domain Layer (Core)
│   ├── Entities/                           # Domain entities
│   │   ├── Transaction.cs
│   │   ├── Dispute.cs
│   │   ├── Customer.cs
│   │   └── DisputeAttachment.cs
│   ├── ValueObjects/                       # Value objects
│   ├── Common/                             # Base classes, exceptions
│   └── Repositories/                       # Repository interfaces
├── TransactionsDisputePortal.Infrastructure/ # Infrastructure Layer
│   ├── Persistence/                        # EF Core, DbContext
│   │   ├── Configurations/                 # Entity configurations
│   │   ├── Migrations/                     # EF migrations
│   │   └── DatabaseSeeder.cs               # Seed data
│   ├── Repositories/                       # Repository implementations
│   └── Services/                           # External services
│       ├── JwtTokenGenerator.cs
│       ├── PasswordHasher.cs
│       └── LocalFileStorageService.cs
├── TransactionsDisputePortal.Domain.Tests/ # Unit Tests (23 tests)
│   └── Entities/
│       ├── DisputeTests.cs
│       └── TransactionTests.cs
└── TransactionsDisputePortal.UI/           # React Frontend
    ├── src/
    │   ├── components/                     # Reusable components
    │   │   ├── CreateDisputeDialog.tsx
    │   │   └── TransactionDetailsDialog.tsx
    │   ├── pages/                          # Page components
    │   │   ├── Login.tsx
    │   │   ├── Dashboard.tsx
    │   │   ├── Transactions.tsx
    │   │   └── Disputes.tsx
    │   ├── contexts/                       # React contexts (Auth)
    │   └── lib/                            # Utilities (axios)
    └── public/
```

## 🔧 Configuration

### JWT Settings
Configure in `appsettings.json`:
```json
{
  "JwtSettings": {
    "Secret": "your-secret-key-minimum-32-characters-long",
    "Issuer": "TransactionsDisputePortal",
    "Audience": "TransactionsDisputePortalUsers",
    "ExpiryHours": 24
  }
}
```

### File Storage
Files are stored locally in `uploads/` directory by default:
- **Location**: `{ProjectRoot}/uploads/disputes/`
- **Max Size**: 10MB per file
- **Max Files**: 5 per dispute
- **Formats**: Images, PDF, Word documents

### CORS
For development, all origins are allowed. Update in `Program.cs` for production:
```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("https://yourdomain.com")
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});
```

## 📈 Future Enhancements

Potential improvements for production:

- [ ] **Background Jobs** - Automated dispute processing (Hangfire)
- [ ] **Email Notifications** - Alert users of dispute updates
- [ ] **Refresh Tokens** - Improved security for long sessions
- [ ] **Rate Limiting** - API throttling for abuse prevention
- [ ] **Caching** - Redis for improved performance
- [ ] **Audit Logs** - Comprehensive change tracking
- [ ] **Role-Based Access** - Admin, Manager, Customer roles
- [ ] **Export Functionality** - Download reports (CSV, PDF)
- [ ] **Real-time Updates** - SignalR for live notifications
- [ ] **Advanced Analytics** - Charts and trend analysis
- [ ] **Concurrency Tokens** - RowVersion for optimistic concurrency
- [ ] **Integration Tests** - End-to-end API testing
- [ ] **Result Pattern** - Functional error handling
- [ ] **Docker Support** - Containerization ready

## 🎯 Key Takeaways for Interviewers

This project demonstrates:

1. **Clean Architecture** - Proper separation of concerns, dependency inversion
2. **CQRS & MediatR** - Scalable command/query separation  
3. **Domain-Driven Design** - Rich domain models with business logic
4. **Test-Driven Development** - Unit tests for domain logic (23 passing tests)
5. **Modern .NET** - Latest C# features, async/await patterns
6. **API Best Practices** - RESTful design, proper HTTP status codes, validation
7. **Security** - JWT authentication, PBKDF2 password hashing
8. **DevOps Awareness** - Health checks, structured logging (Serilog)
9. **UI/UX** - Responsive design, modern React patterns, Material-UI
10. **Code Quality** - Clean code, SOLID principles, comprehensive documentation

### Technical Interview Discussion Points

**Architecture & Design:**
- Why did you choose Clean Architecture over other patterns?
- How would you handle domain events for cross-aggregate communication?
- What's your strategy for handling eventual consistency?

**Scalability:**
- How would you scale read vs write operations differently?
- What caching strategy would you implement?
- How would you handle high-volume dispute processing?

**Performance:**
- Database indexing strategy (show: IX_Disputes_TransactionId)
- Query optimization techniques
- N+1 problem prevention

**Security:**
- Additional measures needed (refresh tokens, rate limiting, MFA)
- How to prevent dispute fraud?
- API security best practices

**Monitoring & Observability:**
- Application Insights / OpenTelemetry integration
- What metrics would you track?
- Alerting strategy

**Deployment:**
- CI/CD pipeline design
- Blue-green vs canary deployments
- Database migration strategy

**Testing:**
- Integration test strategy
- E2E test approach (Playwright)
- How to test external dependencies?

**Error Handling:**
- Global exception handling middleware
- Result pattern vs exceptions
- Error logging and tracking

**Concurrency:**
- Optimistic vs pessimistic locking
- When to use RowVersion?
- Handling concurrent dispute updates

## 👤 Author

**Sifiso (Thabo Kapiteni)**  
Senior Software Engineer

---

## 📝 Notes

- This project uses **South African merchant brands** (Takealot, Checkers, Woolworths, TFG, etc.)
- Database is automatically seeded with test data on first run
- All timestamps are in UTC
- Currency is ZAR (South African Rand)

## 📚 Documentation

- **API Docs**: Available at `/swagger` when running
- **Health Checks**: `/health` endpoint with detailed status
- **Architecture Diagram**: See above ASCII diagram

---

**Built with ❤️ using Clean Architecture principles**
