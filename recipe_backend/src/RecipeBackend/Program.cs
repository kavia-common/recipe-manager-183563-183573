using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Minimal console logging: Information and above
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Configure Kestrel to bind to 0.0.0.0:3001 by default if no URLs arg/env is provided
// This allows running in environments where the runner doesn't pass --urls.
builder.WebHost.ConfigureKestrel(options =>
{
    // no special Kestrel limits needed for now
});

// Only set URLs if neither command-line nor environment provides them
// ASPNETCORE_URLS or --urls take precedence; otherwise we default to http://0.0.0.0:3001
var hasUrlsFromEnv = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("ASPNETCORE_URLS"));
var hasUrlsFromArgs = args.Any(a => a.StartsWith("--urls", StringComparison.OrdinalIgnoreCase));
if (!hasUrlsFromEnv && !hasUrlsFromArgs)
{
    builder.WebHost.UseUrls("http://0.0.0.0:3001");
}

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Recipe Backend API",
        Version = "v1",
        Description = "Minimal API for recipe management scaffold. This will be extended in subsequent tasks."
    });
});

var app = builder.Build();

// Enable Swagger in Development environment only
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// PUBLIC_INTERFACE
app.MapGet("/", () =>
{
    /** Root endpoint to confirm server is running. */
    return Results.Ok(new { name = "recipe-backend", status = "running", version = "v1" });
})
.WithName("Root")
.WithSummary("Root endpoint")
.WithDescription("Returns basic service info to confirm the server is running.")
.Produces<object>(StatusCodes.Status200OK, "application/json");

// PUBLIC_INTERFACE
app.MapGet("/health", () =>
{
    /** Health check endpoint to verify service is running. */
    return Results.Ok(new { status = "ok" });
})
.WithName("HealthCheck")
.WithSummary("Service health check")
.WithDescription("Returns status ok to confirm service is running.")
.Produces<object>(StatusCodes.Status200OK, "application/json");

app.Run();
