# Local Development Checklist

- [ ] Backend runs without errors
- [ ] Frontend runs without errors
- [ ] Frontend `wwwroot/appsettings.json` points to backend URL
- [ ] Backend CORS allows frontend origin (`https://localhost:5173`, `http://localhost:5173`)
- [ ] Browser console has no connection errors

## Startup Commands

Backend:

```powershell
cd backend/src/RSSFeedReader.API
dotnet run
```

Frontend:

```powershell
cd frontend/src/RSSFeedReader.UI
dotnet run
```
