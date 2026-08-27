using System.Text;
using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.Entities;
using CardiacPatientMonitoring.Api.Middleware;
using CardiacPatientMonitoring.Api.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// =========================================================
// Controllers
// =========================================================

builder.Services.AddControllers();

// =========================================================
// FluentValidation
// =========================================================

builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

// =========================================================
// Database
// =========================================================
// Production / Development:
// SQL Server is used here.
//
// Integration tests:
// CustomWebApplicationFactory replaces this registration
// with Entity Framework Core InMemory database.
// =========================================================

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection"));
});

// =========================================================
// ASP.NET Core Identity
// =========================================================

builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;

        options.User.RequireUniqueEmail = true;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// =========================================================
// JWT Service
// =========================================================

builder.Services.AddScoped<IJwtService, JwtService>();

// =========================================================
// Application Services
// =========================================================
// Appointment business logic is handled by the service layer.

builder.Services.AddScoped<IAppointmentService, AppointmentService>();

// =========================================================
// JWT Configuration
// =========================================================

var jwtSettings =
    builder.Configuration.GetSection("Jwt");

var secretKey =
    jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException(
        "JWT SecretKey is not configured.");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    jwtSettings["Issuer"],

                ValidAudience =
                    jwtSettings["Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            secretKey))
            };
    });

// =========================================================
// Authorization
// =========================================================

builder.Services.AddAuthorization();

// =========================================================
// Swagger
// =========================================================

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(
                "Bearer",
                document)] = []
        });
});

// =========================================================
// Build application
// =========================================================

var app = builder.Build();

// =========================================================
// Global Exception Handling
// =========================================================

app.UseMiddleware<ExceptionHandlingMiddleware>();

// =========================================================
// Swagger
// =========================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// =========================================================
// HTTPS
// =========================================================

app.UseHttpsRedirection();

// =========================================================
// Authentication & Authorization
// =========================================================

app.UseAuthentication();
app.UseAuthorization();

// =========================================================
// Controllers
// =========================================================

app.MapControllers();

// =========================================================
// Database initialization + seed
// =========================================================
// IMPORTANT:
// Do NOT run migrations or normal production seed
// during integration tests.
// CustomWebApplicationFactory handles the test database.
// =========================================================

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();

    var services = scope.ServiceProvider;

    var dbContext =
        services.GetRequiredService<AppDbContext>();

    var userManager =
        services.GetRequiredService<
            UserManager<ApplicationUser>>();

    var roleManager =
        services.GetRequiredService<
            RoleManager<IdentityRole>>();

    await dbContext.Database.MigrateAsync();

    await SeedData.InitializeAsync(
        dbContext,
        userManager,
        roleManager);
}

app.Run();