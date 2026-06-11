using AuthService.API.Extensions;
using AuthService.API.Middlewares;
using AuthService.Infrastructure.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

builder.AddCustomLogging(); //Serilog + Seq

// Configure API Versioning
builder.Services.AddCustomApiVersioning();

// Add DbContext configurations
builder.Services.ConfigureDbConfigurationOptions(builder.Configuration);
builder.Services.AddAuthDbContext();

// Add repository registrations
builder.Services.AddRepositoryRegistrations();

// Add service registrations
builder.Services.AddServiceRegistrations();
builder.Services.AddTokenOptions(builder.Configuration);

builder.Services.AddHttpContextAccessor();
builder.Services.AddClientContext(); // Register ClientContext for accessing client type

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthConfigurations(builder.Configuration);
builder.Services.AddCorsPolicy();
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Seed Default Tenant
await app.SeedDefaultTenantAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors("DevCors");
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseHttpsRedirection();
}
else
{
    app.UseForwardedHeaders();
    app.UseCors("ProdCors");
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseMiddleware<ClientTypeMiddleware>(); // Detecting client type from headers

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));
app.MapControllers();

app.Run();
