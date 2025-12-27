using Microsoft.AspNetCore.Identity;

namespace eShop.Data
{
    public static class SeedData
    {
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
           
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

         
            string[] roleNames = { "Admin", "User" };

            foreach (var roleName in roleNames)
            {
              
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                 
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

         
            var adminUser = await userManager.FindByEmailAsync("admin@eshop.com");
            if (adminUser == null)
            {
                var newAdmin = new ApplicationUser
                {
                    UserName = "admin",
                    Email = "admin@eshop.com",
                    PhoneNumber = "0123456789",
                    Address = "Main Office"
                };

                var createAdmin = await userManager.CreateAsync(newAdmin, "Admin@123");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
        }
    }
}