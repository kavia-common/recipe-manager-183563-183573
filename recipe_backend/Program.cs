using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http.HttpResults;
using RecipeBackend.DTOs;
using RecipeBackend.Models;
using RecipeBackend.Services;

var builder = WebApplication.CreateBuilder(args);

// Application metadata for OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocument(config =>
{
    config.Title = "Recipe Manager API";
    config.Description = "REST API for managing recipes (CRUD and search).";
    config.Version = "v1";
    config.PostProcess = document =>
    {
        document.Info.Contact = new NSwag.OpenApiContact
        {
            Name = "Recipe API",
            Url = "https://example.com"
        };
        document.Tags = new List<NSwag.OpenApiTag>
        {
            new() { Name = "Recipes", Description = "Endpoints for managing recipes" }
        };
    };
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.SetIsOriginAllowed(_ => true)
              .AllowCredentials()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Register repository (in-memory for now)
builder.Services.AddSingleton<IRecipeRepository, InMemoryRecipeRepository>();

var app = builder.Build();

// Use CORS
app.UseCors("AllowAll");

// Configure OpenAPI/Swagger
app.UseOpenApi();
app.UseSwaggerUi(config =>
{
    config.Path = "/docs";
});

// Root health endpoint
app.MapGet("/", () => Results.Ok(new { message = "Healthy" }))
   .WithName("HealthCheck")
   .WithDescription("Simple health check for the Recipe Manager API.")
   .Produces(StatusCodes.Status200OK);

// Recipes endpoints
var recipes = app.MapGroup("/api/recipes").WithTags("Recipes");

// GET /api/recipes
recipes.MapGet("", (IRecipeRepository repo) =>
    {
        var items = repo.GetAll().Select(MapToDto);
        return Results.Ok(items);
    })
    .WithName("GetRecipes")
    .WithSummary("Get all recipes")
    .WithDescription("Returns the full list of recipes.")
    .Produces<IEnumerable<RecipeDto>>(StatusCodes.Status200OK);

// GET /api/recipes/{id}
recipes.MapGet("/{id:guid}", (Guid id, IRecipeRepository repo) =>
    {
        var recipe = repo.GetById(id);
        return recipe is null
            ? Results.NotFound(new { message = "Recipe not found" })
            : Results.Ok(MapToDto(recipe));
    })
    .WithName("GetRecipeById")
    .WithSummary("Get recipe by ID")
    .WithDescription("Returns a single recipe by its unique identifier.")
    .Produces<RecipeDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound);

// GET /api/recipes/search?query=...
recipes.MapGet("/search", (string? query, IRecipeRepository repo) =>
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return Results.BadRequest(new { message = "Query is required." });
        }

        var results = repo.Search(query);
        return Results.Ok(results.Select(MapToDto));
    })
    .WithName("SearchRecipes")
    .WithSummary("Search recipes")
    .WithDescription("Search recipes by title, ingredients, tags, or instructions.")
    .Produces<IEnumerable<RecipeDto>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status400BadRequest);

// POST /api/recipes
recipes.MapPost("", (CreateRecipeDto dto, IRecipeRepository repo) =>
    {
        // Model validation via DataAnnotations
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(dto, serviceProvider: null, items: null);
        if (!Validator.TryValidateObject(dto, context, validationResults, true))
        {
            return Results.ValidationProblem(validationResults
                .GroupBy(e => e.MemberNames.FirstOrDefault() ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage ?? "Invalid").ToArray()));
        }

        var now = DateTime.UtcNow;
        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            Title = dto.Title.Trim(),
            Ingredients = dto.Ingredients.Select(i => i.Trim()).Where(i => !string.IsNullOrWhiteSpace(i)).ToList(),
            Instructions = dto.Instructions.Trim(),
            Tags = dto.Tags.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).ToList(),
            CreatedAt = now,
            UpdatedAt = now
        };

        repo.Add(recipe);
        return Results.Created($"/api/recipes/{recipe.Id}", MapToDto(recipe));
    })
    .WithName("CreateRecipe")
    .WithSummary("Create a new recipe")
    .WithDescription("Creates a new recipe and returns it.")
    .Produces<RecipeDto>(StatusCodes.Status201Created)
    .ProducesValidationProblem();

// PUT /api/recipes/{id}
recipes.MapPut("/{id:guid}", (Guid id, UpdateRecipeDto dto, IRecipeRepository repo) =>
    {
        var validationResults = new List<ValidationResult>();
        var context = new ValidationContext(dto, serviceProvider: null, items: null);
        if (!Validator.TryValidateObject(dto, context, validationResults, true))
        {
            return Results.ValidationProblem(validationResults
                .GroupBy(e => e.MemberNames.FirstOrDefault() ?? string.Empty)
                .ToDictionary(g => g.Key, g => g.Select(e => e.ErrorMessage ?? "Invalid").ToArray()));
        }

        var existing = repo.GetById(id);
        if (existing is null)
        {
            return Results.NotFound(new { message = "Recipe not found" });
        }

        existing.Title = dto.Title.Trim();
        existing.Ingredients = dto.Ingredients.Select(i => i.Trim()).Where(i => !string.IsNullOrWhiteSpace(i)).ToList();
        existing.Instructions = dto.Instructions.Trim();
        existing.Tags = dto.Tags.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        existing.UpdatedAt = DateTime.UtcNow;

        var updated = repo.Update(existing);
        return updated
            ? Results.Ok(MapToDto(existing))
            : Results.Problem("Failed to update recipe.", statusCode: StatusCodes.Status500InternalServerError);
    })
    .WithName("UpdateRecipe")
    .WithSummary("Update an existing recipe")
    .WithDescription("Updates an existing recipe by ID and returns the updated recipe.")
    .Produces<RecipeDto>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .ProducesValidationProblem()
    .Produces(StatusCodes.Status500InternalServerError);

// DELETE /api/recipes/{id}
recipes.MapDelete("/{id:guid}", (Guid id, IRecipeRepository repo) =>
    {
        var deleted = repo.Delete(id);
        return deleted
            ? Results.NoContent()
            : Results.NotFound(new { message = "Recipe not found" });
    })
    .WithName("DeleteRecipe")
    .WithSummary("Delete a recipe")
    .WithDescription("Deletes a recipe by ID.")
    .Produces(StatusCodes.Status204NoContent)
    .Produces(StatusCodes.Status404NotFound);

// Helper mapper
static RecipeDto MapToDto(Recipe r) => new()
{
    Id = r.Id,
    Title = r.Title,
    Ingredients = r.Ingredients,
    Instructions = r.Instructions,
    Tags = r.Tags,
    CreatedAt = r.CreatedAt,
    UpdatedAt = r.UpdatedAt
};

app.Run();