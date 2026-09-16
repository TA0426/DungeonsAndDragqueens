using Microsoft.EntityFrameworkCore;
using OppgaveUkeEnModul3.Core;
using OppgaveUkeEnModul3.Core.Interfaces;
using OppgaveUkeEnModul3.Core.Services;
using OppgaveUkeEnModul3.WebApi.Services;
using OppgaveUkeEnModul3.WebApi.DatabaseContext;
using Microsoft.OpenApi;





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

builder.Services.AddDbContext<StoreMonstersContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IStoreMonstersRepository>(
    provider => provider.GetRequiredService<StoreMonstersContext>());

builder.Services.AddScoped<IStoreMonstersService, StoreMonstersService>();
builder.Services.AddTransient<StoreMonsterBuilder>();

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var database = scope.ServiceProvider
        .GetRequiredService<StoreMonstersContext>();

    database.Database.Migrate();
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();


app.MapControllers();

app.Run();


