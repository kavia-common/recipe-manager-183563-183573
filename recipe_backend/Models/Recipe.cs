using System.ComponentModel.DataAnnotations;

namespace RecipeBackend.Models
{
    /// <summary>
    /// Core domain model representing a recipe.
    /// </summary>
    public class Recipe
    {
        /// <summary>
        /// Unique identifier for the recipe.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Title of the recipe.
        /// </summary>
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// Collection of ingredient strings.
        /// </summary>
        public List<string> Ingredients { get; set; } = new();

        /// <summary>
        /// Instructions or steps to prepare the recipe.
        /// </summary>
        public string Instructions { get; set; } = string.Empty;

        /// <summary>
        /// Tags for categorization and search.
        /// </summary>
        public List<string> Tags { get; set; } = new();

        /// <summary>
        /// Creation timestamp (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Last update timestamp (UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; }
    }
}
