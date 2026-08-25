using CardiacPatientMonitoring.Api.Data;
using CardiacPatientMonitoring.Api.Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace CardiacPatientMonitoring.Tests;

public class CustomWebApplicationFactory
    : WebApplicationFactory<global::Program>
{
    public const string TestSecretKey =
        "ThisIsAValidTestSecretKeyForJwtAuthentication123456789";

    public const string TestIssuer =
        "CardiacPatientMonitoringTest";

    public const string TestAudience =
        "CardiacPatientMonitoringTestClient";

    private const string TestDatabaseName =
        "CardiacPatientMonitoringTestDb";

    protected override void ConfigureWebHost(
        IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // =====================================================
            // Remove the SQL Server DbContext completely
            // =====================================================

            services.RemoveAll<AppDbContext>();

            services.RemoveAll<
                DbContextOptions<AppDbContext>>();

            services.RemoveAll<
                IDbContextOptionsConfiguration<AppDbContext>>();

            // =====================================================
            // Add InMemory database for integration tests
            // =====================================================

            services.AddDbContext<AppDbContext>(options =>
            {
                options.UseInMemoryDatabase(TestDatabaseName);
            });
        });

        // =====================================================
        // Test JWT configuration
        // =====================================================

        builder.UseSetting(
            "Jwt:SecretKey",
            TestSecretKey);

        builder.UseSetting(
            "Jwt:Issuer",
            TestIssuer);

        builder.UseSetting(
            "Jwt:Audience",
            TestAudience);
    }

    protected override IHost CreateHost(
        IHostBuilder builder)
    {
        var host = base.CreateHost(builder);

        // =====================================================
        // Initialize the InMemory test database
        // =====================================================

        using var scope = host.Services.CreateScope();

        var dbContext =
            scope.ServiceProvider
                .GetRequiredService<AppDbContext>();

        dbContext.Database.EnsureDeleted();
        dbContext.Database.EnsureCreated();

        // =====================================================
        // Seed patient required by integration tests
        // =====================================================

        if (!dbContext.Patients.Any(p => p.Id == 1))
        {
            dbContext.Patients.Add(
                new Patient
                {
                    Id = 1,
                    FirstName = "Ahmad",
                    LastName = "Hassan",
                    DateOfBirth = new DateTime(1985, 4, 12),
                    Gender = "Male",
                    PhoneNumber = "0599000001",
                    CreatedAt = DateTime.UtcNow
                });

            dbContext.SaveChanges();
        }

        return host;
    }
}