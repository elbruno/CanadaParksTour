using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.AI;
using OpenAI;
using OntarioParksExplorer.Api.Data;
using OntarioParksExplorer.Api.Services;
using OntarioParksExplorer.Api.Services.AI;

var builder = WebApplication.CreateBuilder(args);

// Add Aspire ServiceDefaults
builder.AddServiceDefaults();

// Add controllers
builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger/Swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "Ontario Parks Explorer API",
        Version = "v1",
        Description = "API for exploring Ontario Provincial Parks with search, filtering, and detailed park information",
        Contact = new()
        {
            Name = "Ontario Parks Explorer Team"
        }
    });

    // Enable XML comments for better Swagger documentation
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        options.IncludeXmlComments(xmlPath);
    }
});

// Configure CORS for Blazor and React frontends
var corsOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
    ?? new[] { "http://localhost:5173", "https://localhost:5173" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontends", policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add DbContext with SQLite
var connectionString = builder.Configuration.GetConnectionString("ParksDb") 
    ?? "Data Source=parks.db";

builder.Services.AddDbContext<ParksDbContext>(options =>
    options.UseSqlite(connectionString));

// Add memory cache for AI responses
builder.Services.AddMemoryCache();

// Add response caching
builder.Services.AddResponseCaching();

// Configure AI Chat Client
var aiConfig = builder.Configuration.GetSection("AI");
var aiApiKey = aiConfig["ApiKey"];
var aiModel = aiConfig["Model"] ?? "gpt-4o-mini";

if (!string.IsNullOrEmpty(aiApiKey))
{
    builder.Services.AddSingleton<IChatClient>(services =>
        new OpenAI.Chat.ChatClient(aiModel, apiKey: aiApiKey).AsIChatClient());
}
else
{
    builder.Services.AddSingleton<IChatClient>(provider => null!);
}

// Register services
builder.Services.AddScoped<IParksService, ParksService>();
builder.Services.AddScoped<IAiService, AiService>();

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ParksDbContext>("database");

var app = builder.Build();

// Ensure database is created and seeded (skip in Testing environment)
if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var context = scope.ServiceProvider.GetRequiredService<ParksDbContext>();
        await context.Database.MigrateAsync();
        
        var seedDataPath = Path.Combine(builder.Environment.ContentRootPath, "..", "seed-data", "parks.json");
        if (File.Exists(seedDataPath))
        {
            await DataSeeder.SeedAsync(context, seedDataPath);
        }
    }
}

// Map Aspire default endpoints (includes health checks)
app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Ontario Parks Explorer API v1");
        options.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();

app.UseResponseCaching();

app.UseCors("AllowFrontends");

app.MapControllers();

app.Run();

// Make the implicit Program class public for testing
public partial class Program { }

