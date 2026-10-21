using Microsoft.EntityFrameworkCore;
using DungeonsAndDragqueens.Core;
using DungeonsAndDragqueens.Core.Interfaces;
using DungeonsAndDragqueens.Core.Services;
using DungeonsAndDragqueens.WebApi.Services;
using DungeonsAndDragqueens.WebApi.DatabaseContext;
using DungeonsAndDragqueens.Core.Models;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;
using System.Text;





var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    options.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
});

builder.Services.AddControllers();


builder.Services.AddScoped<LevelCalculator>();
builder.Services.AddScoped<FightService>();
builder.Services.AddScoped<GameService>();
builder.Services.AddScoped<CharacterService>();
builder.Services.AddScoped<SwordService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<IPasswordHasher<PlayerAccount>, PasswordHasher<PlayerAccount>>();

builder.Services.AddDbContext<StoreMonstersContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStoreMonstersRepository>(
    provider => provider.GetRequiredService<StoreMonstersContext>());

builder.Services.AddScoped<IStoreMonstersService, StoreMonstersService>();
builder.Services.AddTransient<StoreMonsterBuilder>();


var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException("JWT key is missing.");
if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
    throw new InvalidOperationException("JWT key must be at least 32 bytes long.");

var jwtIssuer = builder.Configuration["Jwt:Issuer"]
    ?? throw new InvalidOperationException("JWT issuer is missing.");
var jwtAudience = builder.Configuration["Jwt:Audience"]
    ?? throw new InvalidOperationException("JWT audience is missing.");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();



var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider
        .GetRequiredService<StoreMonstersContext>();

    database.Database.Migrate();
    await GameDataSeeder.SeedAsync(database);
}

app.UseDefaultFiles();
app.UseStaticFiles();
app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Generates swagger.json
    app.UseSwaggerUI(); // Optional traditional swagger UI

    // Add Scalar UI endpoint
    app.MapScalarApiReference(options =>
    {
        options
            .WithOpenApiRoutePattern("/swagger/{documentName}/swagger.json")
            .WithTitle("My API")
            .WithTheme(ScalarTheme.Moon);
    });
}

app.MapControllers();

app.Run();


