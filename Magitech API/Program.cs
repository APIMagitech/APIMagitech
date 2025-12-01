using Microsoft.EntityFrameworkCore;
using MagitechAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Configurar porta para SquareCloud
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");

Console.WriteLine("Magitech API iniciada - Neon DB conectado");

// Adicionar serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar banco de dados
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? "Host=ep-hidden-field-adsjor7g-pooler.c-2.us-east-1.aws.neon.tech;Database=neondb;Username=neondb_owner;Password=npg_seuxhl30YfBP;SSL Mode=Require";
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Configurar CORS para permitir seu site React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",
            "http://localhost:3000",
            "https://seu-site.com"
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configurar pipeline HTTP - Swagger sempre ativo
app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowReact");
app.UseAuthorization();
app.MapControllers();

app.Run();
