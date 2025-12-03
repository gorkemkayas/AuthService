using AuthService.API.Extensions;
using AuthService.API.Middlewares;
using AuthService.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);


// Add DbContext configurations
builder.Services.ConfigureDbConfigurationOptions(builder.Configuration);
builder.Services.AddAuthDbContext();

// Add repository registrations
builder.Services.AddRepositoryRegistrations();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthConfigurations(builder.Configuration);
builder.Services.AddCorsPolicy();

var app = builder.Build();
app.UseCors("AllowKayasSubdomains");
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
