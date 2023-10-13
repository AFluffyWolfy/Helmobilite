using Helmobilite;
using Helmobilite.Models;
using Helmobilite.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
if (builder.Environment.IsDevelopment())
{
    var connectionString = builder.Configuration.GetConnectionString("ConnectionStringsDev") ?? throw new InvalidOperationException("Connection string 'HelmobiliteDbContextConnection' not found.");
    builder.Services.AddDbContext<HelmobiliteDbContext>(options => 
        options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStringsDev"))
    );
    builder.Services.AddScoped<TruckRepository>();
    builder.Services.AddScoped<UserRepository>();
    builder.Services.AddScoped<ClientRepository>();
    builder.Services.AddScoped<DriverRepository>();
    builder.Services.AddScoped<DeliveryRepository>();
    builder.Services.AddScoped<ClientRepository>();
}
else
{
    var connectionString = builder.Configuration.GetConnectionString("ConnectionStringsProd") ?? throw new InvalidOperationException("Connection string 'HelmobiliteDbContextConnection' not found.");
    builder.Services.AddDbContext<HelmobiliteDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ConnectionStringsProd")));
    builder.Services.AddScoped<TruckRepository>();
    builder.Services.AddScoped<UserRepository>();
    builder.Services.AddScoped<ClientRepository>();
    builder.Services.AddScoped<DriverRepository>();
    builder.Services.AddScoped<DeliveryRepository>();
    builder.Services.AddScoped<ClientRepository>();
}

// Add services to the container.
builder.Services.AddControllersWithViews();

// Add Identity Info
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<HelmobiliteDbContext>()
    .AddDefaultUI();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Ajouter pour Identity
app.UseAuthorization();
app.MapRazorPages(); // Ajouter pour Identity

// Seed database
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    DataInitializer.SeedRole(roleManager);
    DataInitializer.Seed(userManager);
}

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();