using RecipeBackend.Models;

namespace RecipeBackend.Services
{
    /// <summary>
    /// Repository abstraction for recipe persistence.
    /// Uses in-memory storage in this implementation.
    /// </summary>
    public interface IRecipeRepository
    {
        // PUBLIC_INTERFACE
        IEnumerable<Recipe> GetAll();

        // PUBLIC_INTERFACE
        Recipe? GetById(Guid id);

        // PUBLIC_INTERFACE
        IEnumerable<Recipe> Search(string query);

        // PUBLIC_INTERFACE
        Recipe Add(Recipe recipe);

        // PUBLIC_INTERFACE
        bool Update(Recipe recipe);

        // PUBLIC_INTERFACE
        bool Delete(Guid id);

        // PUBLIC_INTERFACE
        bool Exists(Guid id);
    }

    /// <summary>
    /// In-memory implementation of IRecipeRepository.
    /// </summary>
    public class InMemoryRecipeRepository : IRecipeRepository
    {
        private readonly List<Recipe> _recipes = new();

        public InMemoryRecipeRepository()
        {
            Seed();
        }

        public IEnumerable<Recipe> GetAll() => _recipes.OrderByDescending(r => r.UpdatedAt);

        public Recipe? GetById(Guid id) => _recipes.FirstOrDefault(r => r.Id == id);

        public IEnumerable<Recipe> Search(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<Recipe>();

            query = query.Trim().ToLowerInvariant();
            return _recipes.Where(r =>
                r.Title.ToLowerInvariant().Contains(query) ||
                r.Ingredients.Any(i => i.ToLowerInvariant().Contains(query)) ||
                r.Tags.Any(t => t.ToLowerInvariant().Contains(query)) ||
                r.Instructions.ToLowerInvariant().Contains(query)
            );
        }

        public Recipe Add(Recipe recipe)
        {
            _recipes.Add(recipe);
            return recipe;
        }

        public bool Update(Recipe recipe)
        {
            var idx = _recipes.FindIndex(r => r.Id == recipe.Id);
            if (idx < 0) return false;
            _recipes[idx] = recipe;
            return true;
        }

        public bool Delete(Guid id)
        {
            var existing = GetById(id);
            if (existing is null) return false;
            _recipes.Remove(existing);
            return true;
        }

        public bool Exists(Guid id) => _recipes.Any(r => r.Id == id);

        private void Seed()
        {
            if (_recipes.Any()) return;

            var now = DateTime.UtcNow;

            _recipes.Add(new Recipe
            {
                Id = Guid.NewGuid(),
                Title = "Classic Spaghetti Bolognese",
                Ingredients = new List<string>
                {
                    "200g spaghetti",
                    "250g ground beef",
                    "1 onion, chopped",
                    "2 cloves garlic, minced",
                    "400g canned tomatoes",
                    "2 tbsp olive oil",
                    "Salt",
                    "Pepper",
                    "Basil"
                },
                Instructions = "Cook spaghetti per package. Sauté onion and garlic in olive oil, add beef and brown. Add tomatoes, simmer 15 minutes. Season and serve over spaghetti with basil.",
                Tags = new List<string> { "italian", "pasta", "dinner" },
                CreatedAt = now,
                UpdatedAt = now
            });

            _recipes.Add(new Recipe
            {
                Id = Guid.NewGuid(),
                Title = "Avocado Toast with Egg",
                Ingredients = new List<string>
                {
                    "2 slices sourdough bread",
                    "1 ripe avocado",
                    "1 egg",
                    "Salt",
                    "Pepper",
                    "Chili flakes",
                    "Lemon juice"
                },
                Instructions = "Toast bread. Mash avocado with salt, pepper, lemon. Fry or poach egg. Spread avocado on toast, top with egg and chili flakes.",
                Tags = new List<string> { "breakfast", "quick", "vegetarian" },
                CreatedAt = now,
                UpdatedAt = now
            });

            _recipes.Add(new Recipe
            {
                Id = Guid.NewGuid(),
                Title = "Chicken Stir-Fry",
                Ingredients = new List<string>
                {
                    "300g chicken breast, sliced",
                    "2 cups mixed vegetables",
                    "2 tbsp soy sauce",
                    "1 tbsp sesame oil",
                    "1 clove garlic, minced",
                    "1 tsp ginger, grated"
                },
                Instructions = "Heat oil in a wok, add garlic and ginger, then chicken, stir-fry until cooked. Add vegetables and soy sauce; cook until crisp-tender.",
                Tags = new List<string> { "asian", "quick", "gluten-free" },
                CreatedAt = now,
                UpdatedAt = now
            });
        }
    }
}
