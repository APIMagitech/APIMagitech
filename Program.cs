using Microsoft.EntityFrameworkCore;
using MeuSiteAPI.Data;

var builder = WebApplication.CreateBuilder(args);

Console.WriteLine("Connection String: " + builder.Configuration.GetConnectionString("DefaultConnection"));

// Adicionar serviços
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar banco de dados
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configurar CORS para permitir seu site React
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins(
            "http://localhost:5173",      // React local (Create React App)
            "http://localhost:5173",      // React local (Vite)
            "https://seu-site.com"        // SEU SITE EM PRODUÇÃO (substitua!)
        )
        .AllowAnyMethod()
        .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configurar pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReact");
app.UseAuthorization();
app.MapControllers();

app.Run();