# Data Model: MVP Subscription Management

**Purpose**: Define data structures and entities for subscription management feature.

**Version**: 1.0 | **Date**: 2026-03-08

---

## Core Entities

### Subscription

Represents a single RSS/Atom feed subscription URL.

| Field | Type | Constraints | Notes |
|-------|------|-----------|-------|
| `Id` | int | Primary key, auto-increment | Uniquely identifies subscription in session |
| `Url` | string | Max 2000 chars, non-null, trimmed | Stored as-is, assumed valid by user; no format validation in MVP |
| `AddedAt` | DateTime | UTC, auto-set on creation | Used for ordering list (oldest first or newest first TBD) |

**Notes**:
- No validation of URL format (user provides valid RSS/Atom feed URLs per assumption)
- Frontend enforces max 2000 chars, backend also validates
- No description, title, or other metadata in MVP
- No category/folder assignment in MVP (deferred to Extended-MVP)

### Relationships

**In MVP**: None (single-user, no multi-tenant relationships).

**Future (Extended-MVP+)**:
- Subscription → FeedItems (one-to-many, for feed fetching)
- Subscription → User (many-to-one, when multi-user added)

---

## Data Structures (Implementation Reference)

### Backend - C# Models

```csharp
/// <summary>
/// Represents a single RSS/Atom feed subscription.
/// </summary>
public class Subscription
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}
```

### API DTOs (Request/Response)

```csharp
/// <summary>
/// Request to add a new subscription.
/// </summary>
public class AddSubscriptionRequest
{
    /// <summary>
    /// RSS/Atom feed URL. Max 2000 characters.
    /// </summary>
    [Required]
    [StringLength(2000, MinimumLength = 1)]
    public string Url { get; set; } = string.Empty;
}

/// <summary>
/// Response containing all current subscriptions.
/// </summary>
public class SubscriptionsResponse
{
    /// <summary>
    /// List of subscription URLs in order they were added.
    /// </summary>
    public IEnumerable<SubscriptionDto> Subscriptions { get; set; } = new List<SubscriptionDto>();
}

/// <summary>
/// Subscription data transfer object for API responses.
/// </summary>
public class SubscriptionDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}
```

### Frontend - C# Models

```csharp
/// <summary>
/// Frontend representation of a subscription.
/// </summary>
public class SubscriptionDto
{
    public int Id { get; set; }
    public string Url { get; set; } = string.Empty;
    public DateTime AddedAt { get; set; }
}
```

---

## Storage Layer (Repository Pattern)

### ISubscriptionRepository Interface

```csharp
/// <summary>
/// Abstraction for subscription data access.
/// Supports future migration to database without changing service layer.
/// </summary>
public interface ISubscriptionRepository
{
    /// <summary>
    /// Gets all subscriptions in order they were added.
    /// </summary>
    Task<IEnumerable<Subscription>> GetAllAsync();

    /// <summary>
    /// Adds a new subscription URL.
    /// </summary>
    /// <param name="url">Trimmed, validated URL from client</param>
    Task AddAsync(string url);
}
```

### InMemorySubscriptionRepository Implementation

```csharp
/// <summary>
/// In-memory subscription repository for MVP (single session).
/// Subscriptions are discarded when application restarts.
/// </summary>
public class InMemorySubscriptionRepository : ISubscriptionRepository
{
    private static readonly List<Subscription> _subscriptions = new();
    private static int _nextId = 1;

    public Task<IEnumerable<Subscription>> GetAllAsync()
    {
        return Task.FromResult(_subscriptions.AsEnumerable());
    }

    public Task AddAsync(string url)
    {
        var subscription = new Subscription
        {
            Id = _nextId++,
            Url = url,
            AddedAt = DateTime.UtcNow
        };
        _subscriptions.Add(subscription);
        return Task.CompletedTask;
    }
}
```

**Design Decision**: Static collection allows simple in-memory storage. Production migration to database will create DatabaseSubscriptionRepository using same interface.

---

## Constraints & Validation Rules

### Input Validation

**Frontend (Blazor)**:
- Max length: 2000 characters
- Trim whitespace from start/end
- Reject if empty or whitespace-only
- Display validation error to user (e.g., "URL cannot be empty")

**Backend (ASP.NET Core)**:
- Validate max length: 2000 characters
- Reject if null or empty after trimming
- Log suspicious patterns (very long URL, unusual characters) for debugging
- Return HTTP 400 Bad Request with error detail if validation fails

### Business Rules

- **Allow Duplicates**: Users can add the same URL multiple times in MVP (de-duplication deferred to Extended-MVP)
- **URL Format**: No validation of URL format or RSS/Atom feed validity (assumed valid)
- **Ordering**: Subscriptions displayed in order added (oldest first)
- **Persistence**: Subscriptions not persisted; lost on application restart (by design for MVP)

---

## State Transitions

**Subscription Lifecycle**:
```
[Added] → [Stored in Memory] → [Displayed in List] → [Lost on App Restart]
```

**No State Management**: Subscriptions are immutable once added. No editing, archiving, or status tracking in MVP.

---

## Future Data Model Extensions (Extended-MVP & Post-MVP)

As the application grows, the data model can be extended:

### Extended-MVP
- **FeedItem**: Parsed item from feed (title, link, description)
- Add fields to Subscription: lastRefreshedAt, itemCount

### Post-MVP
- **User**: Multi-user support, authentication
- **Folder**: Subscription organization
- **ReadStatus**: Track read/unread items
- Add fields to Subscription: categoryId, notes, isActive

All these extensions can be added to the database schema without changing the API contract (add new optional fields, gradual migration).

---

## Migration Path to Persistence (Extended-MVP)

When persistence is needed:

1. **Create DatabaseSubscriptionRepository**:
   - Implement ISubscriptionRepository interface
   - Use EF Core with SQLite
   - Same public methods as InMemorySubscriptionRepository

2. **Update Program.cs DI**:
   ```csharp
   // MVP: services.AddScoped<ISubscriptionRepository, InMemorySubscriptionRepository>();
   // Extended-MVP: services.AddScoped<ISubscriptionRepository, DatabaseSubscriptionRepository>();
   ```

3. **Add EF Core migrations**:
   - UpdateDatabase to create tables
   - No changes to SubscriptionService required
   - No API contract changes needed

The decoupled design ensures this migration is straightforward.
