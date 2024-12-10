using System.Collections.Immutable;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Amazon.Runtime;
using Amazon.SimpleEmailV2;
using ApexCharts;
using dotenv.net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MudBlazor.Services;
using OpenLaunch.Components;
using OpenLaunch.Components.Account;
using OpenLaunch.Features.Unsubscribe;
using OpenLaunch.Features.Unsubscribe.Services;
using OpenLaunch.Features.Upload;
using OpenLaunch.Interfaces;
using OpenLaunch.Data;
using OpenLaunch.Endpoints.EmailBounces;
using OpenLaunch.Features.Subscribe;
using OpenLaunch.Filters;
using OpenLaunch.Models;
using OpenLaunch.Services;
using Radzen;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Compact;
using Serilog.Formatting.Json;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

if (environment != "Production")
{
    var options = new DotEnvOptions(envFilePaths: new[] { "../../.env" });
    DotEnv.Load(options);

}

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Warning) // suppresses EF Core query logging
    .MinimumLevel.Override("Microsoft.AspNetCore.Hosting", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Mvc", LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.AspNetCore.Routing", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console(new RenderedCompactJsonFormatter())
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSerilog();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();

var serviceType = builder.Configuration["USE_SERVICE"]?.ToUpper() ?? "MOCK";

Log.Information("Service type {serviceType} detected.", serviceType);

switch (serviceType)
{
    case "AWS":
        builder.Services.AddScoped<AmazonSimpleEmailServiceV2Client>(sp =>
        {
            var accessKey = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var secretKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            var region = Environment.GetEnvironmentVariable("AWS_DEFAULT_REGION");
    
            if (string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(region))
            {
                throw new InvalidOperationException("AWS credentials or region are not set in the environment variables.");
            }

            var awsCredentials = new Amazon.Runtime.BasicAWSCredentials(accessKey, secretKey);
            var awsRegion = Amazon.RegionEndpoint.GetBySystemName(region);

            return new AmazonSimpleEmailServiceV2Client(awsCredentials, awsRegion);
        });
        builder.Services.AddScoped<IEmailService, AWSEmailService>();
        builder.Services.AddScoped<IExternalDataFetching, AWSDataFetching>();
        break;
    
    case "MOCK":
        builder.Services.AddScoped<IEmailService, MockEmailService>();
        builder.Services.AddScoped<IExternalDataFetching, MockDataFetching>();
        break;
    
    default:
        Log.Warning("Build failed due to unknown service type: {serviceType}", serviceType);
        throw new InvalidOperationException($"Unsupported service type: {serviceType}");
}


builder.Services.AddScoped<AdminUserSeeder>();
builder.Services.AddScoped<FromAddressService>();
builder.Services.AddScoped<EmailTemplateService>();
builder.Services.AddScoped<ApiKeyService>();
builder.Services.AddScoped<ApiKeyFilter>();
builder.Services.AddSingleton<DarkModeService>();
builder.Services.AddScoped<BounceService>();

// Unsubscribe Endpoint Services
builder.Services.AddScoped<UnsubscribeHandler>();
builder.Services.AddScoped<UnsubscribeTokenService>();
builder.Services.AddScoped<UnsubscribeLinkService>();

// Upload Endpoint Services
builder.Services.AddScoped<UploadHandler>();

// Subscribe Endpoint Services
builder.Services.AddScoped<SubscribeHandler>();

// Bounce Endpoint Services
builder.Services.AddScoped<SnsBounceHandler>();

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", policy => policy.RequireRole("Admin"));
});

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=/Database/app.db";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireDigit = false;                
        options.Password.RequireUppercase = false; 
        options.SignIn.RequireConfirmedAccount = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddSignInManager()
    .AddDefaultTokenProviders();


builder.Services.AddMudServices();
builder.Services.AddApexCharts();
builder.Services.AddRadzenComponents();

builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.MapHealthChecks("/api/health");

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

// Map Endpoints in /Endpoints
UnsubscribeEndpoint.MapUnsubscribeEndpoints(app);
app.MapUploadEndpoints();
app.MapSubscribeEndpoints();
app.MapBounceEndpoints();

using (var scope = app.Services.CreateScope())
{
    var themeService = scope.ServiceProvider.GetRequiredService<ThemeService>();
    themeService.SetTheme("material");
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<AppDbContext>();
    
    try
    {
        var pendingMigrations = dbContext.Database.GetPendingMigrations().ToList();
        
        if (pendingMigrations.Any())
        {
            Log.Information("Applying pending migrations...");
            dbContext.Database.Migrate();
        }
        else
        {
            Log.Information("Database is already up-to-date.");
        }
    }
    catch (Exception ex)
    {
        Log.Error(ex, "An error occurred while checking or applying migrations.");
        throw;
    }
}

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var adminSeeder = services.GetRequiredService<AdminUserSeeder>();
    
    await adminSeeder.SeedAdminUserAsync();
}

try
{
    Log.Information("Starting App.");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "App terminated unexpectedly.");
}
finally
{
    Log.CloseAndFlush();
}