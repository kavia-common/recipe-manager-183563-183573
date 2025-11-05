# Running the Recipe Backend

You can run from the recipe_backend root without specifying --project:

- Default port (when ASPNETCORE_URLS is not set): http://0.0.0.0:3001

Commands:
- dotnet run
- dotnet run --urls "http://0.0.0.0:3001"

Environment override:
- export ASPNETCORE_URLS="http://0.0.0.0:3001"
- dotnet run

Swagger is enabled only in Development. Health endpoint is available at GET /health and returns { "status": "ok" }.
