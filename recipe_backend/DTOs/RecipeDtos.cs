using System.ComponentModel.DataAnnotations;

namespace RecipeBackend.DTOs
{
    /// <summary>
    /// DTO used to create a new recipe.
    /// </summary>
    public class CreateRecipeDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "At least one ingredient is required.")]
        public List<string> Ingredients { get; set; } = new();

        [Required]
        [MinLength(10, ErrorMessage = "Instructions must be at least 10 characters.")]
        public string Instructions { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// DTO used to update an existing recipe.
    /// </summary>
    public class UpdateRecipeDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MinLength(1, ErrorMessage = "At least one ingredient is required.")]
        public List<string> Ingredients { get; set; } = new();

        [Required]
        [MinLength(10, ErrorMessage = "Instructions must be at least 10 characters.")]
        public string Instructions { get; set; } = string.Empty;

        public List<string> Tags { get; set; } = new();
    }

    /// <summary>
    /// DTO returned to clients for recipes.
    /// </summary>
    public class RecipeDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public List<string> Ingredients { get; set; } = new();
        public string Instructions { get; set; } = string.Empty;
        public List<string> Tags { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
