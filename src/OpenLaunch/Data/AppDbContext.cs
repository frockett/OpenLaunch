using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenLaunch.Models;

namespace OpenLaunch.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<WaitlistSignup> WaitlistSignups { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        // List<IdentityRole> roles = new List<IdentityRole>
        // {
        //     new IdentityRole {
        //         Name = "Admin",
        //         NormalizedName = "ADMIN"
        //     }
        // };
        //
        // builder.Entity<IdentityRole>().HasData(roles);
    }
}