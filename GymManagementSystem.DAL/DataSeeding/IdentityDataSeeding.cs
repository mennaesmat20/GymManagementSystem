using GymManagementSystem.DAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace GymManagementSystem.DAL.DataSeeding
{
    public static class IdentityDataSeeding
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager, ILogger logger, CancellationToken ct = default)
        {
            try
            {
                bool HasUsers = userManager.Users.Any();
                bool HasRoles = roleManager.Roles.Any();

                if (HasUsers && HasRoles) return;

                if (!HasRoles)
                {
                    var Roles = new List<IdentityRole>() {
                        new IdentityRole(){Name = "SuperAdmin"},
                        new IdentityRole(){Name = "Admin"}
                    };

                    foreach (var roleName in Roles.Select(R => R.Name))
                    {
                        if (!await roleManager.RoleExistsAsync(roleName!))
                        {
                            var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName!));
                            if (!roleResult.Succeeded)
                            {
                                logger.LogError("Failed to Create Role...");
                                return;
                            }
                        }
                    }
                }
                if (!HasUsers)
                {
                    var MainAdmin = new ApplicationUser()
                    {
                        FirstName = "Menna",
                        LastName = "Esmat",
                        UserName = "MennaEsmat",
                        Email = "mennaesmat@gmail.com",
                        PhoneNumber = "01225474200"
                    };

                    await userManager.CreateAsync(MainAdmin, "P@ssw0rd");
                    await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");

                    var Admin01 = new ApplicationUser()
                    {
                        FirstName = "Ahmed",
                        LastName = "Ali",
                        UserName = "AhmedAli",
                        Email = "AhmedAli@gmail.com",
                        PhoneNumber = "01269855478"
                    };

                    var CreateResult = await userManager.CreateAsync(Admin01, "P@ssw0rd");

                    if (!CreateResult.Succeeded)
                    {
                        logger.LogError("Failed to Seed Users");
                        return;
                    }
                    await userManager.AddToRoleAsync(Admin01, "Admin");
                }
                return;
            }
            catch (Exception ex)
			{
                logger.LogError(ex, "Identity seeding failed.");
                throw;
            }
        }
    }
}
