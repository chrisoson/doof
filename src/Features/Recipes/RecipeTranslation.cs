using Microsoft.EntityFrameworkCore;

namespace doof.Features.Recipes;

public class RecipeTranslation
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }
    public required string Language { get; set; }
    public required string RecipeTitle { get; set; }
    public string? RecipeSubTitle { get; set; }
    public ICollection<RecipeStep> Steps { get; set; } = [];

    public static void Configure(ModelBuilder builder)
    {
        builder.Entity<RecipeTranslation>(e =>
        {
            e.HasKey(rt => rt.Id);

            e.Property(r => r.Language).HasMaxLength(10);
            e.Property(r => r.RecipeTitle).HasMaxLength(50);
            e.Property(r => r.RecipeSubTitle).HasMaxLength(200);
        });
    }
}