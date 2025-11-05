using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Minimal console logging: Information and above
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Configure Kestrel explicitly and ensure it binds to 0.0.0.0 when no explicit URL provided
builder.WebHost.ConfigureKestrel(options =>
{
    // No special limits; reserve the spot to customize if needed.
});

// Explicit URL selection precedence:
// 1) Command line --urls
// 2) ASPNETCORE_URLS env var
// 3) Fallback to http://0.0.0.0:3001
// Note: Some orchestrators inject args without --urls; rely on env var or fallback accordingly.
var urlsFromEnv = Environment.GetEnvironmentVariable("ASPNETCORE_URLS");
var hasUrlsFromEnv = !string.IsNullOrWhiteSpace(urlsFromEnv);
var hasUrlsFromArgs = args.Any(a => a.StartsWith("--urls", StringComparison.OrdinalIgnoreCase));

if (hasUrlsFromEnv)
{
    // Respect ASPNETCORE_URLS if provided
    builder.WebHost.UseUrls(urlsFromEnv!);
}
else if (!hasUrlsFromArgs)
{
    // Default binding for containerized environments if neither env nor --urls is provided
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

// Log the bound URLs and environment at startup to aid diagnostics
var logger = app.Services.GetRequiredService<ILoggerFactory>().CreateLogger("Startup");
logger.LogInformation("Environment: {Env}", app.Environment.EnvironmentName);
logger.LogInformation("ASPNETCORE_URLS = {UrlsEnv}", Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "(null)");
logger.LogInformation("Server listening on: {Addresses}", string.Join(", ", app.Urls));

// Provide an explicit readiness log once app has started.
// Note: app.Urls is populated after server starts; this log happens right before Run() blocking call.
logger.LogInformation("Recipe backend starting. Health endpoint available at {HealthUrlHint}", 
    (Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? "http://0.0.0.0:3001").TrimEnd('/') + "/health");

app.Run();
