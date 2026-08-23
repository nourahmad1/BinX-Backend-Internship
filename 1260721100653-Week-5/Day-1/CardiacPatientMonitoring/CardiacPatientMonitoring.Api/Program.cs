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

// Add controller support
builder.Services.AddControllers();

// Register FluentValidation for request validation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddFluentValidationAutoValidation();

// Connect the API to the SQL Server database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure ASP.NET Core Identity for user management
builder.Services
    .AddIdentityCore<ApplicationUser>(options =>
    {
        // Password rules
        options.Password.RequireDigit = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequiredLength = 6;

        // Each user must have a unique email
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

// Register the JWT service
builder.Services.AddScoped<IJwtService, JwtService>();

// Read JWT settings from appsettings.json
var jwtSettings = builder.Configuration.GetSection("Jwt");

var secretKey = jwtSettings["SecretKey"]
    ?? throw new InvalidOperationException(
        "JWT SecretKey is not configured.");

// Configure JWT authentication
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

                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],

                // Use the secret key to verify the token signature
                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(secretKey))
            };
    });

builder.Services.AddAuthorization();

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    // Add JWT authentication to Swagger
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

var app = builder.Build();

// Handle unexpected errors in one place
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Enable Swagger only while developing the API
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Redirect HTTP requests to HTTPS
app.UseHttpsRedirection();

// Authentication must run before authorization
app.UseAuthentication();
app.UseAuthorization();

// Map controller endpoints
app.MapControllers();

// Apply database migrations and add the initial sample data
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext =
        services.GetRequiredService<AppDbContext>();

    var userManager =
        services.GetRequiredService<UserManager<ApplicationUser>>();

    await dbContext.Database.MigrateAsync();

    await SeedData.InitializeAsync(
        dbContext,
        userManager);
}

app.Run();