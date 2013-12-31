using doof.Features.Recipes;
using Microsoft.AspNetCore.Identity;

namespace doof.Features.Users;

public class CustomUser : IdentityUser
{
    public ICollection<Recipe> Recipes { get; set; } = [];
}