# Quick Start: MVP Subscription Management

**Purpose**: Get the MVP subscription management feature up and running quickly for development and testing.

**Version**: 1.0 | **Date**: 2026-03-08

**Audience**: Developers implementing the MVP feature.

---

## Prerequisites

- **.NET 8 SDK** or later installed ([download](https://dotnet.microsoft.com/download/dotnet/8.0))
- **Visual Studio Code** or **Visual Studio 2022** (recommended)
- **Git** for version control
- Cross-platform: Works on Windows, macOS, Linux

**Estimated Setup Time**: 10-15 minutes first run, <1 minute on subsequent runs.

---

## Project Setup

### 1. Create Project Structure

```powershell
cd RSSFeedReader

# Create backend project
dotnet new webapi -n backend/RSSFeedReader.API -f net8.0
dotnet new xunit -n backend/RSSFeedReader.API.Tests -f net8.0

# Create frontend project
dotnet new blazorwasm -n frontend/RSSFeedReader.UI -f net8.0
dotnet new xunit -n frontend/RSSFeedReader.UI.Tests -f net8.0

# Create solution file
dotnet new sln -n RSSFeedReader
dotnet sln RSSFeedReader.sln add backend/RSSFeedReader.API/RSSFeedReader.API.csproj
dotnet sln RSSFeedReader.sln add backend/RSSFeedReader.API.Tests/RSSFeedReader.API.Tests.csproj
dotnet sln RSSFeedReader.sln add frontend/RSSFeedReader.UI/RSSFeedReader.UI.csproj
dotnet sln RSSFeedReader.sln add frontend/RSSFeedReader.UI.Tests/RSSFeedReader.UI.Tests.csproj
```

### 2. Backend Configuration

**File**: `backend/RSSFeedReader.API/Program.cs`

```csharp
var builder = WebApplicationBuilder.CreateBuilder(args);

// Add services
builder.Services.AddControllers();
builder.Services.AddScoped<ISubscriptionRepository, InMemorySubscriptionRepository>();
builder.Services.AddScoped<SubscriptionService>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevelopmentPolicy", policy =>
    {
        policy
            .WithOrigins("https://localhost:5173", "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.UseHttpsRedirection();
app.UseCors("DevelopmentPolicy");
app.MapControllers();

app.Run();
```

**Key Points**:
- CORS explicitly configured for frontend origin
- In-memory repository injected
- No authentication needed for MVP

### 3. Frontend Configuration

**File**: `frontend/RSSFeedReader.UI/wwwroot/appsettings.json`

```json
{
  "ApiUrl": "http://localhost:5000"
}
```

**Note**: Update to https://localhost:5001 or production URL as needed.

---

## Running the Application

### Terminal 1: Start Backend API

```powershell
cd backend/RSSFeedReader.API
dotnet run
```

**Expected Output**:
```
Building...
...
Now listening on: http://localhost:5000
...
Application started. Press Ctrl+C to shut down.
```

**Port**: http://localhost:5000 (HTTP) or https://localhost:5001 (HTTPS)

### Terminal 2: Start Frontend

```powershell
cd frontend/RSSFeedReader.UI
dotnet run
```

**Expected Output**:
```
Building...
...
Now listening on: https://localhost:5173
...
Application started. Press Ctrl+C to shut down.
```

**URL**: Open https://localhost:5173 in browser

---

## Testing the MVP

### Manual Test: Add Subscription

1. **Open browser**: https://localhost:5173
2. **See**: Subscriptions page with input field and empty list
3. **Enter URL**: `https://devblogs.microsoft.com/dotnet/feed/`
4. **Click**: "Add Subscription" button
5. **Verify**: URL appears in list below
6. **Repeat**: Add 2-3 more subscriptions
7. **Verify**: All subscriptions display in order added

### Automated Test: Run Unit Tests

```powershell
# Backend tests
cd backend/RSSFeedReader.API.Tests
dotnet test

# Frontend tests
cd frontend/RSSFeedReader.UI.Tests
dotnet test
```

**Expected**: All tests PASS (green output)

---

## Verification Checklist

Before considering MVP ready, verify:

- [ ] Backend API runs without errors on port 5000/5001
- [ ] Frontend loads without errors on port 5173 in browser
- [ ] Frontend configuration (`appsettings.json`) points to correct backend URL
- [ ] CORS allows frontend origin (check DevTools Network tab)
- [ ] Browser DevTools console shows no connection errors
- [ ] Can add subscription and see it appear in list within 1 second
- [ ] Can add 10+ subscriptions without errors
- [ ] Subscriptions display in order added
- [ ] Backend unit tests pass (70%+ coverage)
- [ ] Frontend component tests pass
- [ ] One end-to-end test passes (add subscription → see in list)

---

## Troubleshooting

### Issue: "Connection Refused" (Frontend → Backend)

**Cause**: Backend not running or CORS not configured.

**Solution**:
1. Verify backend running: `dotnet run` in backend/RSSFeedReader.API
2. Verify CORS policy includes frontend origin (https://localhost:5173)
3. Check DevTools Network tab to see actual request/response

### Issue: Certificate Error (HTTPS)

**Cause**: Self-signed development certificate not trusted.

**Solution**:
```powershell
dotnet dev-certs https --trust
```

Then restart backend.

### Issue: Port Already in Use

**Cause**: Another process using port 5000, 5001, or 5173.

**Solution**:
```powershell
# Find process using port 5000 (Windows)
netstat -ano | findstr :5000

# Kill process by PID
taskkill /PID {PID} /F

# Or use different port
dotnet run --urls "http://localhost:6000"
```

### Issue: Build Fails

**Cause**: .NET SDK mismatch or missing dependencies.

**Solution**:
```powershell
dotnet --version  # Should be 8.0+

dotnet restore   # Restore NuGet packages

dotnet clean    # Clean and rebuild
dotnet build
```

---

## Development Workflow

### Daily Development Loop

```powershell
# Terminal 1: Backend
cd backend/RSSFeedReader.API
dotnet watch run    # Auto-restarts on file changes

# Terminal 2: Frontend
cd frontend/RSSFeedReader.UI
dotnet watch run    # Auto-restarts on file changes

# Terminal 3: Run tests on change
cd backend/RSSFeedReader.API.Tests
dotnet watch test
```

### Making Changes

1. **Edit code** (automatically recompiles)
2. **Browser auto-refresh** shows changes
3. **Tests run automatically** (watch mode)
4. **Commit changes** with clear messages

---

## Clean Up / Reset

### Reset to Fresh State

```powershell
# Remove all in-memory subscriptions by restarting backend
# (Ctrl+C to stop, then `dotnet run` to restart)

# Clear browser cache
# DevTools → Application tab → Clear storage
```

### Full Clean

```powershell
# Remove all build outputs
dotnet clean

# Remove NuGet cache (optional)
dotnet nuget locals all --clear

# Rebuild
dotnet build
```

---

## Next Steps (Extended-MVP)

Once MVP subscription management is working:

1. **Add feed fetching**:
   - Add System.ServiceModel.Syndication NuGet package
   - Create FeedFetcher service
   - Add manual refresh endpoint

2. **Add item display**:
   - Create FeedItem model
   - Parse feed items in backend

3. **Add persistence**:
   - Add Entity Framework Core with SQLite
   - Create database migrations
   - Implement DatabaseSubscriptionRepository

4. **Improve UI**:
   - Add error handling UI
   - Add delete subscription button
   - Better styling / layout

See [plan.md](plan.md) for full technical roadmap.

---

## Additional Resources

- [ASP.NET Core Documentation](https://learn.microsoft.com/aspnet/core)
- [Blazor WebAssembly Documentation](https://learn.microsoft.com/aspnet/core/blazor)
- [.NET CLI Commands](https://learn.microsoft.com/dotnet/core/tools)
- [xUnit Testing Framework](https://xunit.net)

---

## Support

For questions or issues:
1. Check this troubleshooting section
2. Review [data-model.md](data-model.md) for data structure details
3. Review [contracts/api-contract.md](contracts/api-contract.md) for API details
4. Consult project constitution for code quality standards
