# recipe-manager-183563-183573

Backend container: recipe_backend
- .NET 8 ASP.NET Core Web API
- Swagger enabled in Development
- Health endpoint at GET /health returns { "status": "ok" }
- Root endpoint at GET / returns basic service info

Run:
- From repo root:
  dotnet run --project recipe_backend/src/RecipeBackend/RecipeBackend.csproj --urls "http://0.0.0.0:3001"
- Or from recipe_backend root:
  dotnet run
  (defaults to http://0.0.0.0:3001 if ASPNETCORE_URLS is not set)

Environment variable option:
- export ASPNETCORE_URLS="http://0.0.0.0:3001" && dotnet run
