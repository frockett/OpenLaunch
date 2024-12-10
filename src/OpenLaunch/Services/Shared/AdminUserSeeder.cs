using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OpenLaunch.Models;
using Serilog;

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
        Log.Information("Checking for users...");
        
        if (await _userManager.Users.AnyAsync())
        {
            Log.Information("User already exists, cancel user seeding procedure...");
            return;
        }
        
        Log.Information("No users found. Attempting to see admin user.");
        var adminUsername = Environment.GetEnvironmentVariable("ADMIN_USERNAME") ?? "admin";
        var adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "password";
        
        if (!await _roleManager.RoleExistsAsync("Admin"))
        {
            await _roleManager.CreateAsync(new IdentityRole("Admin"));
        }
        
        var adminUser = await _userManager.FindByNameAsync(adminUsername);
        if (adminUser == null)
        {
            Log.Information("No admin user {username} found. Creating new admin user.", adminUsername);
            adminUser = new AppUser { UserName = adminUsername, Email = $"{adminUsername}@example.com" , EmailConfirmed = true};
            var result = await _userManager.CreateAsync(adminUser, adminPassword);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }
            Log.Information("Admin user created.");
        }
        else
        {
            Log.Information("Admin user with name {username} already exists.", adminUsername);
        }
    }
}