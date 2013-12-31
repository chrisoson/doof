using Microsoft.EntityFrameworkCore;

namespace doof.Features.Recipes;

public class RecipeTranslation
{
    public int Id { get; set; }
    public int RecipeId { get; set; }
    public required string Language { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }

    public Recipe Recipe { get; set; }

    public static void Configure(ModelBuilder builder)
    {
        builder.Entity<RecipeTranslation>(e =>
        {
            e.HasKey(rt => rt.Id);

            e.Property(r => r.Language).HasMaxLength(10);
        });
    }
}