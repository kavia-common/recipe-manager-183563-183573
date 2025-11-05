# Running the Recipe Backend

You can run from the recipe_backend root without specifying --project.

- Default binding (when ASPNETCORE_URLS and --urls are not set): http://0.0.0.0:3001
- Health endpoint: GET /health returns { "status": "ok" }
- Root endpoint: GET / returns basic service info

Commands:
- dotnet run
- dotnet run --urls "http://0.0.0.0:3001"

Environment override:
- export ASPNETCORE_URLS="http://0.0.0.0:3001"
- dotnet run

Notes:
- The app logs environment and bound URLs at startup to help diagnostics.
- Ensure port 3001 is exposed by your container/orchestrator.

Swagger is enabled only in Development.
