using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenLaunch.Components.Account.Pages.Manage;
using OpenLaunch.Models;

namespace OpenLaunch.Data;

public class AppDbContext : IdentityDbContext<AppUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    
    public DbSet<WaitlistSignup> WaitlistSignups { get; set; }
    public DbSet<FromAddress> FromAddresses { get; set; }
    public DbSet<EmailTemplate> EmailTemplates { get; set; }
    public DbSet<ApiKey> ApiKeys { get; set; }
    public DbSet<Bounces> Bounces { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
    }
}
