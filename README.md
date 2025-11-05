# recipe-manager-183563-183573

Recipe Backend (.NET 8) provides RESTful endpoints to manage recipes with in-memory storage so it runs without external dependencies.

- Swagger UI: http://localhost:3001/docs
- OpenAPI JSON: http://localhost:3001/openapi.json
- Health: http://localhost:3001/

Endpoints:
- GET /api/recipes — list all recipes
- GET /api/recipes/{id} — get a recipe by ID
- GET /api/recipes/search?query=term — search by title, ingredients, tags, or instructions
- POST /api/recipes — create a recipe
- PUT /api/recipes/{id} — update a recipe
- DELETE /api/recipes/{id} — delete a recipe

Request/Response Models:
- CreateRecipeDto, UpdateRecipeDto require:
  {
    "title": "string (<=200 chars)",
    "ingredients": ["string", "..."],
    "instructions": "string (>=10 chars)",
    "tags": ["string", "..."]
  }

- RecipeDto returns all fields plus:
  {
    "id": "guid",
    "createdAt": "ISO-8601 UTC",
    "updatedAt": "ISO-8601 UTC"
  }

Quick examples (curl):
- List:
  curl -s http://localhost:3001/api/recipes

- Search:
  curl -s "http://localhost:3001/api/recipes/search?query=chicken"

- Create:
  curl -s -X POST http://localhost:3001/api/recipes \
    -H "Content-Type: application/json" \
    -d '{
      "title": "Grilled Cheese Sandwich",
      "ingredients": ["2 slices bread", "2 slices cheese", "butter"],
      "instructions": "Butter bread. Place cheese between slices. Grill until golden and cheese melts.",
      "tags": ["quick","lunch","vegetarian"]
    }'

- Update:
  curl -s -X PUT http://localhost:3001/api/recipes/{id} \
    -H "Content-Type: application/json" \
    -d '{
      "title": "Grilled Cheese Sandwich (Crispy)",
      "ingredients": ["2 slices bread", "2 slices cheese", "butter"],
      "instructions": "Butter bread generously. Place cheese between slices. Grill on medium-low until deeply golden and cheese melts.",
      "tags": ["quick","lunch","vegetarian"]
    }'

- Delete:
  curl -s -X DELETE http://localhost:3001/api/recipes/{id}