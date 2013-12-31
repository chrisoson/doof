using Microsoft.EntityFrameworkCore;

namespace doof.Features.Recipes;

public class RecipeStep
{
    public int Id { get; set; }
    public int RecipeTranslationId { get; set; }
    public RecipeTranslation RecipeTranslation { get; set; }
    public required int StepNumber { get; set; }
    public required string Description { get; set; }

    public static void Configure(ModelBuilder builder)
    {
        builder.Entity<RecipeStep>(e =>
        {
            e.HasKey(rs => rs.Id);
            e.Property(rs => rs.Description).HasMaxLength(500).IsRequired();
            e.Property(rs => rs.StepNumber).HasMaxLength(50).IsRequired();
            e.HasOne(rs => rs.RecipeTranslation)
                .WithMany(rt => rt.Steps)
                .HasForeignKey(rs => rs.RecipeTranslationId);
        });
    }
}