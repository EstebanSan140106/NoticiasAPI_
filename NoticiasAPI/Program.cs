using Microsoft.EntityFrameworkCore;
using NoticiasAPI.Context;
using NoticiasAPI.Entities;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configurar Entity Framework
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cadenaSQL")));

// Configurar CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ⚠️ IMPORTANTE: El orden de estos middlewares es crucial
app.UseDefaultFiles();       // Debe ir ANTES de UseStaticFiles
app.UseStaticFiles();        // Debe ir DESPUÉS de UseDefaultFiles

app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();