using doof.Features.Users;
using Microsoft.EntityFrameworkCore;

namespace doof.Features.Recipes;

public class Recipe
{
    public int Id { get; set; }

    public required string AuthorId { get; set; }
    public CustomUser CustomUser { get; set; }

    public ICollection<RecipeImage> Images { get; set; }

    public ICollection<RecipeTranslation> Translations { get; set; } = [];

    public static void Configure(ModelBuilder builder)
    {
        builder.Entity<Recipe>(e =>
        {
            e.HasKey(r => r.Id);

            e.Property(r => r.AuthorId).IsRequired();

            e.HasMany(r => r.Translations)
                .WithOne(r => r.Recipe)
                .HasForeignKey(r => r.RecipeId);

            e.HasMany(r => r.Images)
                .WithOne(ri => ri.Recipe)
                .HasForeignKey(ri => ri.RecipeId);

            e.HasOne(r => r.CustomUser)
                .WithMany(u => u.Recipes)
                .HasForeignKey(r => r.AuthorId);
        });
    }
}