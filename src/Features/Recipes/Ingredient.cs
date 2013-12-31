using Microsoft.EntityFrameworkCore;

namespace doof.Features.Recipes;

public class Ingredient
{
    public int Id { get; set; }

    public ICollection<IngredientTranslation> Translations { get; set; } = [];

    public static void Configure(ModelBuilder builder)
    {
        builder.Entity<Ingredient>(e =>
        {
            e.HasKey(i => i.Id);
        });
    }
}