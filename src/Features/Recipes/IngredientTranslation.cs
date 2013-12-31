using Microsoft.EntityFrameworkCore;

namespace doof.Features.Recipes;

public class IngredientTranslation
{
    public int Id { get; set; }

    public int IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;

    public required string Language { get; set; }

    public required string IngredientName { get; set; }

    public static void Configure(ModelBuilder builder)
    {
        builder.Entity<IngredientTranslation>(e =>
        {
            e.HasKey(it => it.Id);
            e.Property(it => it.Language).IsRequired().HasMaxLength(10);
            e.Property(it => it.IngredientName).IsRequired().HasMaxLength(50);
            e.HasOne(it => it.Ingredient)
                .WithMany(i => i.Translations)
                .HasForeignKey(it => it.IngredientId);
        });
    }
}