using Blog;
using Blog.Extensions;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


builder.LoadConfiguration();

var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
builder.Services.LoadConfigAuthentication(key);

builder
    .Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddEndpointsApiExplorer();

builder.Services.LoadConfigSwagger();

builder.Services.LoadConfigDataContext(builder.Configuration);

builder.Services.LoadDependecyInjections();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();
app.MapControllers();

app.Run();