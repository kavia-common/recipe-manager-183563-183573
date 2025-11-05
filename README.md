# recipe-manager-183563-183573

Backend container: recipe_backend
- .NET 8 ASP.NET Core Web API
- Swagger enabled in Development
- Health endpoint at GET /health returns { "status": "ok" }

Run:
dotnet run --project recipe_backend/src/RecipeBackend/RecipeBackend.csproj --urls "http://0.0.0.0:3001"
