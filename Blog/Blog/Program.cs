using Blog.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

#pragma warning disable CS8600 // Convers�o de literal nula ou poss�vel valor nulo em tipo n�o anul�vel.
string CONNECTION_STRING_ID = Environment.GetEnvironmentVariable("CONNECTION_STRING_SQL_SERVER_ID", EnvironmentVariableTarget.Machine);
#pragma warning restore CS8600 // Convers�o de literal nula ou poss�vel valor nulo em tipo n�o anul�vel.
#pragma warning disable CS8600 // Convers�o de literal nula ou poss�vel valor nulo em tipo n�o anul�vel.
string CONNECTION_STRING_PASSWORD = Environment.GetEnvironmentVariable("CONNECTION_STRING_SQL_SERVER_PASSWORD", EnvironmentVariableTarget.Machine);
#pragma warning restore CS8600 // Convers�o de literal nula ou poss�vel valor nulo em tipo n�o anul�vel.

builder.Services.AddDbContext<BlogDataContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetSection("ConnectionString").Value
        .Replace("CONNECTION_STRING_ID", CONNECTION_STRING_ID)
        .Replace("CONNECTION_STRING_PASSWORD", CONNECTION_STRING_PASSWORD))
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
