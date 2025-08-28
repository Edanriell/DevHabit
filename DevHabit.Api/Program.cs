using DevHabit.Api;
using DevHabit.Api.Extensions;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder
    .AddApiServices()
    .AddErrorHandling()
    .AddDatabase()
    .AddObservability()
    .AddApplicationServices()
    .AddAuthenticationServices();

WebApplication app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    await app.ApplyMigrationsAsync();

    await app.SeedInitialDataAsync();
}

app.UseHttpsRedirection();

app.UseExceptionHandler();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

// Add-Migration Add_Habits -Context ApplicationDbContext
// Add-Migration Add_Habits -o Migrations/Application
// Add-Migration Add_Identity -Context ApplicationIdentityDbContext -o Migrations/Identity
// dotnet ef migrations add Add_HabitTags --output-dir Migrations/Application
// dotnet ef migrations add Add_Identity --context ApplicationIdentityDbContext --output-dir Migrations/Identity

// RABC - Role Based Access Control
// [Authorize("users:read")]
// ABAC - Attribute Based Access Control, Casbin
// https://github.com/casbin/casbin
// Examples ! 
// RBAC: "All managers can view financial reports"
// ABAC: "Users can view financial reports if they are in the finance department, during business hours, and from company IP addresses
