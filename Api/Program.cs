using System.Text;
using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
 
using MotoX.Application;
using MotoX.Domain.Entities;
using MotoX.Infrastructure;
 
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


// Add services to the container.
builder.Services.AddControllers();
// Clean Architecture
builder.Services.AddApplication();

builder.Services.AddApplication();       
builder.Services.AddInfrastructure(builder.Configuration);  // DbContext
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
 


builder.Services.AddHttpContextAccessor();
// Program.cs یا Startup.cs



builder.Services.AddScoped<IBikeRepository, BikeRepository>();
builder.Services.AddScoped<IAdvertisementRepository, AdvertisementRepository>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddScoped<IContactRepository, ContactRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Swagger

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowNext",
        policy => policy
            .AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod());
});


builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
            ),
            RoleClaimType = System.Security.Claims.ClaimTypes.Role
        };
    });


builder.Services.AddEndpointsApiExplorer();

builder.Services.AddIdentityCore<ApplicationUser>(options => {
    options.User.RequireUniqueEmail = true;
})
.AddRoles<IdentityRole>() // این خط برای پشتیبانی از نقش‌ها حیاتی است
.AddEntityFrameworkStores<AppDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
builder.Services.AddHostedService<UserCleanupWorker>();
builder.Services.AddAuthorization();
builder.Services.AddSingleton<EmailService>();

builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("auth-policy", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 7; 
        opt.QueueLimit = 0;
    });
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(5000); // HTTP
    
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
 
}


app.UseCors("AllowNext");
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Uploads")),
    RequestPath = "/Uploads"
});
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();  

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

        string[] roleNames = { "Admin", "User" };
        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        var adminEmail = "rotex2021rx@gmail.com"; 
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var rootUser = new ApplicationUser
            {
                UserName = "Admin",
                Email = adminEmail,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow
            };
            var create = await userManager.CreateAsync(rootUser, "Admin@1234");
            if (create.Succeeded)
            {
                var newUser = await userManager.FindByEmailAsync(adminEmail);
                await userManager.AddToRoleAsync(newUser, "Admin");
            }
            
        }

        if (adminUser != null && !await userManager.IsInRoleAsync(adminUser, "Admin"))
        {
            await userManager.AddToRoleAsync(adminUser, "Admin");
        }
        

    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while seeding the database.");
    }
}

app.Run();
 
