using AuthService.API.Extensions;
using AuthService.API.Middlewares;
using AuthService.Infrastructure.Extensions;

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

var app = builder.Build();
app.UseCors("AllowKayasSubdomains");

// Seed Default Tenant
await app.SeedDefaultTenantAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseMiddleware<ClientTypeMiddleware>(); // Detecting client type from headers

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
