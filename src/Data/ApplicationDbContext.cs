using doof.Features.Recipes;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace doof.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<Recipe> Recipes { get; set; }
    public DbSet<RecipeImage> RecipeImages { get; set; }
    public DbSet<RecipeTranslation> RecipeTranslations { get; set; }
    public DbSet<Ingredient> Ingredients { get; set; }
    public DbSet<IngredientTranslation> IngredientTranslations { get; set; }
    public DbSet<Tag> Tags { get; set; }
    public DbSet<TagTranslation> TagTranslations { get; set; }
    public DbSet<RecipeStep> RecipeSteps { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        Recipe.Configure(builder);
        RecipeTranslation.Configure(builder);
        RecipeImage.Configure(builder);
        Ingredient.Configure(builder);
        IngredientTranslation.Configure(builder);
        Tag.Configure(builder);
        TagTranslation.Configure(builder);
        RecipeStep.Configure(builder);
    }
}