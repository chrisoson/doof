using Microsoft.EntityFrameworkCore;

namespace doof.Features.Recipes;

public class RecipeImage
{
    public int Id { get; set; }
    public required string Url { get; set; }
    public int RecipeId { get; set; }
    public Recipe Recipe { get; set; }

    public static void Configure(ModelBuilder builder)
    {
        builder.Entity<RecipeImage>(e =>
        {
            e.HasKey(ri => ri.Id);

            e.Property(ri => ri.Url).HasMaxLength(50);
        });
    }
}