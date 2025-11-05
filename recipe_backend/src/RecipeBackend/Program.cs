using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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

// Enable Swagger in Development environment
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
