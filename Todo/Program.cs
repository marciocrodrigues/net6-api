using Microsoft.EntityFrameworkCore;
using Todo.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite("DataSource=app.db;Cache=Shared"));

var app = builder.Build();

app.MapControllers();

app.Run();
