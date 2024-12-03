using Microsoft.AspNetCore.Identity;
using OpenLaunch.Models;

namespace OpenLaunch.Services;

public class AdminUserSeeder
{
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AdminUserSeeder(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task SeedAdminUserAsync()
    {
        var adminUsername = Environment.GetEnvironmentVariable("ADMIN_USERNAME") ?? "admin";
        var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "password";
        
        Console.WriteLine("Seeding admin user");
        
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            Console.WriteLine("NO ROLE EXISTS, MAKE IT");
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        else
        {
            Console.WriteLine("ADMIN ROLE ALREADY EXISTS");
        }
        
        var adminUser = await _userManager.FindByNameAsync(adminUsername);
        if (adminUser == null)
        {
            Console.WriteLine("THE USER DOESN'T EXIST, MAKE IT");
            adminUser = new AppUser { UserName = adminUsername, Email = $"{adminUsername}@example.com" , EmailConfirmed = true};
            var result = await _userManager.CreateAsync(adminUser, adminPassword);
            Console.WriteLine($"RESULT: {result.ToString()}");
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }
    }
}