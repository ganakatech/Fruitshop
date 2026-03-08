# Fruit Shop Pricing System

A .NET 8 Web API application for managing fruits and calculating order prices with support for multiple pricing strategies.

## Overview

This system demonstrates a clean architecture implementation using multiple design patterns to create a flexible, maintainable, and testable fruit shop pricing system. The application supports different pricing models (per kg, per item, with discounts) and can easily be extended to support new pricing strategies.

## Features

- **Fruit Management**: CRUD operations for managing fruits
- **Flexible Pricing**: Support for multiple pricing strategies (per kg, per item, discounted)
- **Order Calculation**: Calculate total order prices with mixed fruit types and pricing strategies
- **Validation**: Comprehensive input validation using FluentValidation
- **Logging**: Request/response logging and performance monitoring
- **API Documentation**: Swagger/OpenAPI documentation
- **Health Checks**: Database and system health monitoring
- **Unit Tests**: Comprehensive test coverage using XUnit

## Architecture

The system uses three main design patterns:

1. **CQRS Pattern**: Separates read operations (queries) from write operations (commands) for better separation of concerns
2. **Strategy Pattern**: Encapsulates different pricing algorithms in interchangeable strategy classes
3. **Factory Pattern**: Creates fruit instances with appropriate pricing strategies based on configuration

### Architecture Flow

```
API Endpoints → MediatR → Command/Query Handlers → Services → Factory → Pricing Strategies
                                                      ↓
                                                  DbContext → SQLite In-Memory DB
```

## Design Patterns Used

### 1. CQRS (Command Query Responsibility Segregation)

**Why**: Separates read and write operations, making the system more maintainable and scalable. Commands handle all write operations (Create, Update, Delete) while queries handle all read operations (Get, GetAll, Calculate).

**Implementation**:
- Commands: `CreateFruitCommand`, `UpdateFruitCommand`, `DeleteFruitCommand`
- Queries: `GetFruitQuery`, `GetAllFruitsQuery`, `CalculateOrderTotalQuery`
- MediatR is used for dispatching commands and queries to their respective handlers

**Benefits**:
- Clear separation of concerns
- Easy to add new operations without modifying existing code
- Better testability
- Can scale read and write operations independently

### 2. Strategy Pattern

**Why**: Allows easy addition of new pricing models without modifying existing code, following the Open/Closed Principle.

**Implementation**:
- `IPricingStrategy` interface defines the contract
- Concrete strategies: `PerKgPricingStrategy`, `PerItemPricingStrategy`, `DiscountedPricingStrategy`
- Strategies are interchangeable and can be composed (e.g., Discounted wraps another strategy)

**Benefits**:
- Easy to add new pricing strategies
- No modification of existing code required
- Strategies can be tested independently
- Supports composition (discounted strategy wraps base strategies)

### 3. Factory Pattern

**Why**: Centralizes fruit creation logic and ensures correct strategy assignment. Also reconstructs strategies from database-stored type.

**Implementation**:
- `FruitFactory` static class with `Create` and `ReconstructStrategy` methods
- Creates fruits with appropriate pricing strategies based on configuration
- Reconstructs strategies when loading from database

**Benefits**:
- Single point of creation logic
- Ensures consistency in strategy assignment
- Handles strategy reconstruction from database
- Easy to extend for new strategy types

## Technology Stack

- **.NET 8**: Latest LTS version of .NET
- **Entity Framework Core 8.0**: ORM with Code First approach
- **SQLite In-Memory**: Database for simplicity (can be extended to file-based or other providers)
- **MediatR**: CQRS implementation library
- **FluentValidation**: Input validation
- **AutoMapper**: Entity-to-DTO mapping
- **Swashbuckle**: Swagger/OpenAPI documentation
- **XUnit**: Unit testing framework
- **FluentAssertions**: Fluent test assertions

## Project Structure

```
FruitShop.Api/
├── Data/                    # EF Core DbContext and configurations
├── Models/                  # Entity models (Fruit, Order, OrderItem)
├── DTOs/                    # Data Transfer Objects
├── Mappings/                # AutoMapper profiles
├── Strategies/              # Pricing strategy implementations
├── Factories/               # Fruit factory
├── Services/                # Business logic services
├── Commands/                # CQRS commands and handlers
├── Queries/                 # CQRS queries and handlers
├── Validators/              # FluentValidation validators
├── Behaviors/               # MediatR pipeline behaviors
└── Program.cs               # Application entry point

FruitShop.Api.Tests/
├── Strategies/              # Strategy tests
├── Services/                # Service tests
├── Factories/               # Factory tests
├── Commands/                # Command handler tests
├── Queries/                 # Query handler tests
└── Validators/              # Validator tests
```

## API Endpoints

### Fruits

- `GET /fruits` - Get all fruits
- `GET /fruits/{id}` - Get fruit by ID
- `POST /fruits` - Create a new fruit
- `PUT /fruits/{id}` - Update an existing fruit
- `DELETE /fruits/{id}` - Delete a fruit

### Orders

- `POST /orders/calculate` - Calculate total price of an order

### Health

- `GET /health` - Health check endpoint

### Documentation

- `GET /swagger` - Swagger UI (development only)

## Example Usage

### Create a Fruit

```json
POST /fruits
{
  "name": "Apple",
  "basePrice": 2.00,
  "pricingStrategyType": "PerKg"
}
```

### Create a Fruit with Discount

```json
POST /fruits
{
  "name": "Cherry",
  "basePrice": 5.00,
  "pricingStrategyType": "Discounted",
  "discountThreshold": 2.0,
  "discountPercentage": 10.0
}
```

### Calculate Order Total

```json
POST /orders/calculate
{
  "items": [
    {
      "fruitId": 1,
      "amount": 2.0
    },
    {
      "fruitId": 2,
      "amount": 5.0
    }
  ]
}
```

## Running the Application

1. **Restore packages**:
   ```bash
   dotnet restore
   ```

2. **Run the API**:
   ```bash
   cd FruitShop.Api
   dotnet run
   ```

3. **Access Swagger UI**:
   Navigate to `https://localhost:5001/swagger` (or the port shown in the console)

4. **Run tests**:
   ```bash
   dotnet test
   ```

## Design Decisions

### 1. SQLite In-Memory Database

**Decision**: Use SQLite in-memory database for simplicity.

**Rationale**: 
- No file persistence needed for this demo
- Easy to set up and test
- Can be easily extended to file-based SQLite or other databases

**Extension**: Change connection string in `Program.cs` to switch to file-based or other providers.

### 2. Pricing Strategy Persistence

**Decision**: Store pricing strategy type as string in database, reconstruct strategy objects when loading.

**Rationale**:
- `IPricingStrategy` is an interface and cannot be serialized
- Strategy type and discount parameters stored separately
- Strategies reconstructed using factory pattern

### 3. Unified Amount Field

**Decision**: Use a single `Amount` field in `OrderItem` that represents either weight or quantity.

**Rationale**:
- Simplifies API - system determines whether amount represents weight or quantity based on fruit's pricing strategy
- Reduces complexity in order creation
- Strategy determines the unit of measurement

### 4. DTOs for API Responses

**Decision**: All API endpoints return DTOs instead of entities.

**Rationale**:
- Prevents exposing internal entity structure
- Allows versioning of API contracts
- Better control over serialization
- AutoMapper handles mapping automatically

### 5. MediatR Pipeline Behaviors

**Decision**: Use pipeline behaviors for validation and logging.

**Rationale**:
- Cross-cutting concerns handled automatically
- No need to add validation/logging code in each handler
- Easy to add new behaviors (caching, authorization, etc.)

## Extension Points

The system is designed with extensibility in mind. Here are detailed extension points with step-by-step implementation guidance:

### 1. Adding New Pricing Strategies

The Strategy Pattern makes it easy to add new pricing models without modifying existing code.

#### Step-by-Step Implementation:

**Step 1: Create the Strategy Class**

Create a new class in `FruitShop.Api/Strategies/` implementing `IPricingStrategy`:

```csharp
namespace FruitShop.Api.Strategies;

public class BulkPricingStrategy : IPricingStrategy
{
    private readonly decimal _bulkThreshold;
    private readonly decimal _bulkDiscountPercentage;

    public BulkPricingStrategy(decimal bulkThreshold, decimal bulkDiscountPercentage)
    {
        _bulkThreshold = bulkThreshold;
        _bulkDiscountPercentage = bulkDiscountPercentage;
    }

    public decimal CalculatePrice(decimal basePrice, decimal amount)
    {
        var totalPrice = basePrice * amount;
        
        // Apply bulk discount if threshold is met
        if (amount >= _bulkThreshold)
        {
            var discount = totalPrice * (_bulkDiscountPercentage / 100m);
            totalPrice -= discount;
        }
        
        return totalPrice;
    }

    public string GetUnit()
    {
        return "kg"; // or "item" depending on your use case
    }
}
```

**Step 2: Update FruitFactory**

Add the new strategy type to `FruitFactory.cs` in both `Create` and `ReconstructStrategy` methods:

```csharp
// In Create method, add to the switch expression:
"Bulk" => new BulkPricingStrategy(
    discountThreshold ?? throw new ArgumentException("Bulk threshold required"),
    discountPercentage ?? throw new ArgumentException("Bulk discount percentage required")
),

// In ReconstructStrategy method, add the same case:
"Bulk" => new BulkPricingStrategy(
    fruit.DiscountThreshold ?? throw new ArgumentException("Bulk threshold required"),
    fruit.DiscountPercentage ?? throw new ArgumentException("Bulk discount percentage required")
),
```

**Step 3: Update Validators**

Update `CreateFruitCommandValidator.cs` and `UpdateFruitCommandValidator.cs`:

```csharp
// Update BeValidPricingStrategyType method:
private bool BeValidPricingStrategyType(string? pricingStrategyType)
{
    if (string.IsNullOrWhiteSpace(pricingStrategyType))
    {
        return false;
    }

    return pricingStrategyType == "PerKg" || 
           pricingStrategyType == "PerItem" || 
           pricingStrategyType == "Discounted" ||
           pricingStrategyType == "Bulk"; // Add new type
}

// Add validation rules for Bulk strategy (similar to Discounted):
When(x => x.PricingStrategyType == "Bulk", () =>
{
    RuleFor(x => x.DiscountThreshold)
        .NotNull().WithMessage("Bulk threshold is required for Bulk pricing strategy")
        .GreaterThan(0).WithMessage("Bulk threshold must be greater than 0");

    RuleFor(x => x.DiscountPercentage)
        .NotNull().WithMessage("Bulk discount percentage is required for Bulk pricing strategy")
        .GreaterThan(0).WithMessage("Bulk discount percentage must be greater than 0")
        .LessThanOrEqualTo(100).WithMessage("Bulk discount percentage cannot exceed 100");
});
```

**Step 4: Update DTO Documentation**

Update XML comments in `CreateFruitRequest.cs` and `UpdateFruitRequest.cs` to include the new strategy type in the `PricingStrategyType` property documentation.

**Step 5: Add Unit Tests**

Create `BulkPricingStrategyTests.cs` in `FruitShop.Api.Tests/Strategies/`:

```csharp
using FluentAssertions;
using FruitShop.Api.Strategies;

namespace FruitShop.Api.Tests.Strategies;

public class BulkPricingStrategyTests
{
    [Fact]
    public void CalculatePrice_BelowThreshold_ShouldReturnNormalPrice()
    {
        // Arrange
        var strategy = new BulkPricingStrategy(10.0m, 15.0m);
        decimal basePrice = 2.00m;
        decimal amount = 5.0m;

        // Act
        decimal totalPrice = strategy.CalculatePrice(basePrice, amount);

        // Assert
        totalPrice.Should().Be(10.00m); // 2.00 * 5.0
    }

    [Fact]
    public void CalculatePrice_AtThreshold_ShouldApplyDiscount()
    {
        // Arrange
        var strategy = new BulkPricingStrategy(10.0m, 15.0m);
        decimal basePrice = 2.00m;
        decimal amount = 10.0m;

        // Act
        decimal totalPrice = strategy.CalculatePrice(basePrice, amount);

        // Assert
        // 2.00 * 10.0 = 20.00, 15% discount = 3.00, total = 17.00
        totalPrice.Should().Be(17.00m);
    }
}
```

**Benefits:**
- No modification to existing pricing strategies required
- Easy to test in isolation
- Can be composed with other strategies (like DiscountedPricingStrategy)
- Follows Open/Closed Principle

### 2. Adding New Commands/Queries

The CQRS pattern makes it straightforward to add new operations.

#### Step-by-Step Implementation for a New Command:

**Step 1: Create the Command**

Create `ArchiveFruitCommand.cs` in `FruitShop.Api/Commands/`:

```csharp
using MediatR;
using FruitShop.Api.DTOs;

namespace FruitShop.Api.Commands;

public class ArchiveFruitCommand : IRequest<FruitDto>
{
    public int Id { get; set; }
    public bool Archived { get; set; }
}
```

**Step 2: Create the Command Handler**

Create `ArchiveFruitCommandHandler.cs`:

```csharp
using MediatR;
using AutoMapper;
using FruitShop.Api.DTOs;
using FruitShop.Api.Services;

namespace FruitShop.Api.Commands;

public class ArchiveFruitCommandHandler : IRequestHandler<ArchiveFruitCommand, FruitDto>
{
    private readonly IFruitService _fruitService;
    private readonly IMapper _mapper;

    public ArchiveFruitCommandHandler(IFruitService fruitService, IMapper mapper)
    {
        _fruitService = fruitService;
        _mapper = mapper;
    }

    public async Task<FruitDto> Handle(ArchiveFruitCommand request, CancellationToken cancellationToken)
    {
        var fruit = await _fruitService.GetByIdAsync(request.Id);
        if (fruit == null)
        {
            throw new KeyNotFoundException($"Fruit with ID {request.Id} not found.");
        }

        // Update archived status (you may need to add Archived property to Fruit entity)
        // fruit.Archived = request.Archived;
        var updatedFruit = await _fruitService.UpdateAsync(fruit);
        
        return _mapper.Map<FruitDto>(updatedFruit);
    }
}
```

**Step 3: Create Validator (Optional but Recommended)**

Create `ArchiveFruitCommandValidator.cs` in `FruitShop.Api/Validators/`:

```csharp
using FluentValidation;
using FruitShop.Api.Commands;

namespace FruitShop.Api.Validators;

public class ArchiveFruitCommandValidator : AbstractValidator<ArchiveFruitCommand>
{
    public ArchiveFruitCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0).WithMessage("Id must be greater than 0");
    }
}
```

**Step 4: Add Endpoint**

Add to `WebApplicationExtensions.cs`:

```csharp
app.MapPatch("/fruits/{id}/archive", async (IMediator mediator, int id, bool archived) =>
{
    var command = new ArchiveFruitCommand { Id = id, Archived = archived };
    var result = await mediator.Send(command);
    return Results.Ok(result);
})
.WithName("ArchiveFruit")
.WithTags("Fruits")
.WithSummary("Archive or unarchive a fruit")
.Produces<FruitDto>(StatusCodes.Status200OK)
.Produces(StatusCodes.Status404NotFound);
```

**Step 5: Add Unit Tests**

Create `ArchiveFruitCommandHandlerTests.cs` in `FruitShop.Api.Tests/Commands/` following the same pattern as existing command handler tests.

#### Step-by-Step Implementation for a New Query:

**Step 1: Create the Query**

Create `GetFruitsByPricingStrategyQuery.cs` in `FruitShop.Api/Queries/`:

```csharp
using MediatR;
using FruitShop.Api.DTOs;

namespace FruitShop.Api.Queries;

public class GetFruitsByPricingStrategyQuery : IRequest<IEnumerable<FruitDto>>
{
    public string PricingStrategyType { get; set; } = string.Empty;
}
```

**Step 2: Create the Query Handler**

Create `GetFruitsByPricingStrategyQueryHandler.cs`:

```csharp
using MediatR;
using AutoMapper;
using FruitShop.Api.DTOs;
using FruitShop.Api.Services;

namespace FruitShop.Api.Queries;

public class GetFruitsByPricingStrategyQueryHandler 
    : IRequestHandler<GetFruitsByPricingStrategyQuery, IEnumerable<FruitDto>>
{
    private readonly IFruitService _fruitService;
    private readonly IMapper _mapper;

    public GetFruitsByPricingStrategyQueryHandler(IFruitService fruitService, IMapper mapper)
    {
        _fruitService = fruitService;
        _mapper = mapper;
    }

    public async Task<IEnumerable<FruitDto>> Handle(
        GetFruitsByPricingStrategyQuery request, 
        CancellationToken cancellationToken)
    {
        var allFruits = await _fruitService.GetAllAsync();
        var filteredFruits = allFruits
            .Where(f => f.PricingStrategyType == request.PricingStrategyType);
        
        return _mapper.Map<IEnumerable<FruitDto>>(filteredFruits);
    }
}
```

**Step 3: Add Endpoint**

Add to `WebApplicationExtensions.cs`:

```csharp
app.MapGet("/fruits/by-strategy/{pricingStrategyType}", async (IMediator mediator, string pricingStrategyType) =>
{
    var query = new GetFruitsByPricingStrategyQuery { PricingStrategyType = pricingStrategyType };
    var result = await mediator.Send(query);
    return Results.Ok(result);
})
.WithName("GetFruitsByPricingStrategy")
.WithTags("Fruits")
.WithSummary("Get fruits by pricing strategy type")
.Produces<List<FruitDto>>(StatusCodes.Status200OK);
```

**Benefits:**
- Clear separation of read and write operations
- Automatic validation through pipeline behaviors
- Easy to add logging, caching, or authorization
- Testable in isolation

### 3. Database Extensions

#### Switching to File-based SQLite

**Step 1: Update Connection String**

In `Program.cs`, change:

```csharp
builder.Services.AddDbContext<FruitShopDbContext>(options =>
    options.UseSqlite("Data Source=:memory:"));
```

To:

```csharp
builder.Services.AddDbContext<FruitShopDbContext>(options =>
    options.UseSqlite("Data Source=fruitshop.db"));
```

**Step 2: Update Database Initialization**

Replace `EnsureCreated()` with migrations:

```csharp
// Remove this:
dbContext.Database.EnsureCreated();

// Add migrations instead:
// 1. Run: dotnet ef migrations add InitialCreate --project FruitShop.Api
// 2. Run: dotnet ef database update --project FruitShop.Api
// 3. In Program.cs:
dbContext.Database.Migrate();
```

#### Switching to SQL Server

**Step 1: Install Package**

```bash
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
```

**Step 2: Update Connection String**

```csharp
builder.Services.AddDbContext<FruitShopDbContext>(options =>
    options.UseSqlServer("Server=localhost;Database=FruitShop;Trusted_Connection=True;"));
```

**Step 3: Update Entity Configuration**

SQL Server may require different data types. Update `FruitConfiguration.cs` if needed:

```csharp
builder.Property(f => f.BasePrice).HasColumnType("decimal(18,2)");
// SQL Server might need: .HasColumnType("money") or .HasColumnType("decimal(10,2)")
```

#### Adding EF Core Migrations

**Step 1: Install EF Core Tools**

```bash
dotnet tool install --global dotnet-ef
```

**Step 2: Create Initial Migration**

```bash
cd FruitShop.Api
dotnet ef migrations add InitialCreate
```

**Step 3: Apply Migration**

```bash
dotnet ef database update
```

**Step 4: Update Program.cs**

Replace `EnsureCreated()` with:

```csharp
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<FruitShopDbContext>();
    dbContext.Database.Migrate();
    SeedData(dbContext);
}
```

### 4. Adding MediatR Pipeline Behaviors

Pipeline behaviors allow you to add cross-cutting concerns like caching, authorization, or performance monitoring.

#### Example: Adding Caching Behavior

**Step 1: Install Package**

```bash
dotnet add package Microsoft.Extensions.Caching.Memory
```

**Step 2: Create Caching Behavior**

Create `CachingBehavior.cs` in `FruitShop.Api/Behaviors/`:

```csharp
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace FruitShop.Api.Behaviors;

public class CachingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IMemoryCache _cache;
    private readonly ILogger<CachingBehavior<TRequest, TResponse>> _logger;

    public CachingBehavior(IMemoryCache cache, ILogger<CachingBehavior<TRequest, TResponse>> logger)
    {
        _cache = cache;
        _logger = logger;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        // Only cache queries, not commands
        if (request is not IRequest<TResponse> query)
        {
            return await next();
        }

        var cacheKey = $"{typeof(TRequest).Name}_{request.GetHashCode()}";
        
        if (_cache.TryGetValue(cacheKey, out TResponse? cachedResponse))
        {
            _logger.LogInformation("Cache hit for {RequestType}", typeof(TRequest).Name);
            return cachedResponse!;
        }

        var response = await next();
        
        _cache.Set(cacheKey, response, TimeSpan.FromMinutes(5));
        _logger.LogInformation("Cached response for {RequestType}", typeof(TRequest).Name);
        
        return response;
    }
}
```

**Step 3: Register Behavior**

In `Program.cs`:

```csharp
builder.Services.AddMemoryCache();
builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(CachingBehavior<,>));
```

**Note:** Order matters! Behaviors are executed in registration order. Place caching before validation if you want to cache before validation.

### 5. Adding Authentication and Authorization

#### Step-by-Step Implementation:

**Step 1: Install Packages**

```bash
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

**Step 2: Configure JWT in Program.cs**

```csharp
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization();
```

**Step 3: Create Authorization Behavior**

Create `AuthorizationBehavior.cs`:

```csharp
using MediatR;
using Microsoft.AspNetCore.Authorization;

namespace FruitShop.Api.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthorizationBehavior(
        IAuthorizationService authorizationService,
        IHttpContextAccessor httpContextAccessor)
    {
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<TResponse> Handle(
        TRequest request, 
        RequestHandlerDelegate<TResponse> next, 
        CancellationToken cancellationToken)
    {
        // Check if request requires authorization
        // You can add attributes or interfaces to mark commands/queries that need authorization
        
        return await next();
    }
}
```

**Step 4: Add Authorization to Endpoints**

```csharp
app.MapPost("/fruits", async (IMediator mediator, CreateFruitRequest request) =>
{
    // ... handler code
})
.RequireAuthorization() // Add this
.WithName("CreateFruit");
```

### 6. Adding Repository Pattern

If you want to abstract data access further, you can add a repository layer.

#### Step-by-Step Implementation:

**Step 1: Create Repository Interface**

Create `IFruitRepository.cs`:

```csharp
namespace FruitShop.Api.Repositories;

public interface IFruitRepository
{
    Task<IEnumerable<Fruit>> GetAllAsync();
    Task<Fruit?> GetByIdAsync(int id);
    Task<Fruit> AddAsync(Fruit fruit);
    Task<Fruit> UpdateAsync(Fruit fruit);
    Task<bool> DeleteAsync(int id);
}
```

**Step 2: Implement Repository**

Create `FruitRepository.cs`:

```csharp
using Microsoft.EntityFrameworkCore;
using FruitShop.Api.Data;
using FruitShop.Api.Models;

namespace FruitShop.Api.Repositories;

public class FruitRepository : IFruitRepository
{
    private readonly FruitShopDbContext _context;

    public FruitRepository(FruitShopDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Fruit>> GetAllAsync()
    {
        return await _context.Fruits.ToListAsync();
    }

    public async Task<Fruit?> GetByIdAsync(int id)
    {
        return await _context.Fruits.FindAsync(id);
    }

    public async Task<Fruit> AddAsync(Fruit fruit)
    {
        _context.Fruits.Add(fruit);
        await _context.SaveChangesAsync();
        return fruit;
    }

    public async Task<Fruit> UpdateAsync(Fruit fruit)
    {
        _context.Fruits.Update(fruit);
        await _context.SaveChangesAsync();
        return fruit;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var fruit = await _context.Fruits.FindAsync(id);
        if (fruit == null)
        {
            return false;
        }

        _context.Fruits.Remove(fruit);
        await _context.SaveChangesAsync();
        return true;
    }
}
```

**Step 3: Update Service to Use Repository**

Update `FruitService.cs`:

```csharp
private readonly IFruitRepository _repository;

public FruitService(IFruitRepository repository)
{
    _repository = repository;
}

public async Task<List<Fruit>> GetAllAsync()
{
    var fruits = await _repository.GetAllAsync();
    return fruits.Select(FruitFactory.ReconstructStrategy).ToList();
}
```

**Step 4: Register Repository**

In `Program.cs`:

```csharp
builder.Services.AddScoped<IFruitRepository, FruitRepository>();
```

**Benefits:**
- Better testability (can mock repository)
- Separation of data access concerns
- Easier to swap data sources

### 7. Adding Unit of Work Pattern

For transaction management across multiple repositories.

#### Step-by-Step Implementation:

**Step 1: Create Unit of Work Interface**

```csharp
namespace FruitShop.Api.Repositories;

public interface IUnitOfWork : IDisposable
{
    IFruitRepository Fruits { get; }
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

**Step 2: Implement Unit of Work**

```csharp
using Microsoft.EntityFrameworkCore.Storage;
using FruitShop.Api.Data;

namespace FruitShop.Api.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly FruitShopDbContext _context;
    private IDbContextTransaction? _transaction;
    private IFruitRepository? _fruits;

    public UnitOfWork(FruitShopDbContext context)
    {
        _context = context;
    }

    public IFruitRepository Fruits
    {
        get
        {
            _fruits ??= new FruitRepository(_context);
            return _fruits;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
```

**Step 3: Update Service to Use Unit of Work**

```csharp
public class FruitService : IFruitService
{
    private readonly IUnitOfWork _unitOfWork;

    public FruitService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Fruit> CreateAsync(Fruit fruit)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var created = await _unitOfWork.Fruits.AddAsync(fruit);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();
            return FruitFactory.ReconstructStrategy(created);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
}
```

### 8. Adding Event Sourcing / Domain Events

For audit trails and event-driven architecture.

#### Step-by-Step Implementation:

**Step 1: Create Domain Event Interface**

```csharp
namespace FruitShop.Api.Events;

public interface IDomainEvent
{
    DateTime OccurredOn { get; }
}
```

**Step 2: Create Domain Events**

```csharp
namespace FruitShop.Api.Events;

public class FruitCreatedEvent : IDomainEvent
{
    public int FruitId { get; set; }
    public string FruitName { get; set; } = string.Empty;
    public DateTime OccurredOn { get; } = DateTime.UtcNow;
}
```

**Step 3: Publish Events from Handlers**

```csharp
public class CreateFruitCommandHandler : IRequestHandler<CreateFruitCommand, FruitDto>
{
    private readonly IMediator _mediator; // Add this

    public async Task<FruitDto> Handle(CreateFruitCommand request, CancellationToken cancellationToken)
    {
        // ... create fruit logic
        
        // Publish event
        await _mediator.Publish(new FruitCreatedEvent 
        { 
            FruitId = createdFruit.Id, 
            FruitName = createdFruit.Name 
        }, cancellationToken);
        
        return result;
    }
}
```

**Step 4: Create Event Handlers**

```csharp
using MediatR;

namespace FruitShop.Api.Events;

public class FruitCreatedEventHandler : INotificationHandler<FruitCreatedEvent>
{
    private readonly ILogger<FruitCreatedEventHandler> _logger;

    public FruitCreatedEventHandler(ILogger<FruitCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(FruitCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fruit created: {FruitName} (ID: {FruitId})", 
            notification.FruitName, notification.FruitId);
        
        // Add other event handling logic (e.g., send email, update cache, etc.)
        
        return Task.CompletedTask;
    }
}
```

### Summary of Extension Benefits

- **Strategy Pattern**: Add new pricing models without touching existing code
- **CQRS**: Add new operations by creating commands/queries, no modification to existing handlers
- **Pipeline Behaviors**: Add cross-cutting concerns (caching, logging, authorization) automatically
- **Repository Pattern**: Abstract data access for better testability
- **Unit of Work**: Manage transactions across multiple operations
- **Domain Events**: Implement event-driven architecture for audit trails and decoupled systems

All extension points follow SOLID principles and maintain backward compatibility with existing code.

## Testing

The project includes comprehensive unit tests using XUnit and FluentAssertions. All tests follow the AAA (Arrange, Act, Assert) pattern.

**Test Coverage**:
- Pricing strategies (PerKg, PerItem, Discounted)
- Factory pattern
- Services (FruitService, OrderService)
- Command handlers (Create, Update, Delete)
- Query handlers (Get, GetAll, Calculate)
- Validators (FluentValidation)

**Run tests**:
```bash
dotnet test
```

## Implemented Extension Points

The following extension points are implemented as part of the core system:

1. **FluentValidation**: All commands have validators with comprehensive validation rules
2. **Logging Behavior**: MediatR pipeline behavior logs all requests/responses with execution time
3. **AutoMapper**: Entity-to-DTO mapping configured with profiles
4. **Response DTOs**: All API endpoints return DTOs instead of entities
5. **Swagger/OpenAPI**: Complete API documentation with XML comments
6. **Health Checks**: Database and system health monitoring endpoint

## Future Enhancements

- Add API versioning
- Implement caching layer
- Add authentication and authorization
- Implement event sourcing for audit trails
- Add repository pattern for better testability
- Implement unit of work pattern for transaction management
- Add integration tests
- Add performance tests

