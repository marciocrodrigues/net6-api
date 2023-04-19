using Blog;
using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// configura��o jwt token
var key = Encoding.ASCII.GetBytes(Configuration.JwtKey);
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder
    .Services.AddControllers()
    .ConfigureApiBehaviorOptions(options =>
    {
        options.SuppressModelStateInvalidFilter = true;
    });

builder.Services.AddEndpointsApiExplorer();

// configura��o swagger com autentica��o Jwt Token
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIContagem", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization Header - utilizado com Bearer Authentication.\r\n\r\n" +
            "Digite 'Bearer' [espa�o] e ent�o seu token no campo abaixo.\r\n\r\n" +
            "Exemplo (informar sem as aspas): 'Bearer 12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

#pragma warning disable CS8600 // Convers�o de literal nula ou poss�vel valor nulo em tipo n�o anul�vel.
string CONNECTION_STRING_ID = Environment.GetEnvironmentVariable("CONNECTION_STRING_SQL_SERVER_ID", EnvironmentVariableTarget.Machine);
#pragma warning restore CS8600 // Convers�o de literal nula ou poss�vel valor nulo em tipo n�o anul�vel.
#pragma warning disable CS8600 // Convers�o de literal nula ou poss�vel valor nulo em tipo n�o anul�vel.
string CONNECTION_STRING_PASSWORD = Environment.GetEnvironmentVariable("CONNECTION_STRING_SQL_SERVER_PASSWORD", EnvironmentVariableTarget.Machine);
#pragma warning restore CS8600 // Convers�o de literal nula ou poss�vel valor nulo em tipo n�o anul�vel.

#pragma warning disable CS8602 // Desrefer�ncia de uma refer�ncia possivelmente nula.
builder.Services.AddDbContext<BlogDataContext>(options =>
    options.UseSqlServer(builder.Configuration
        .GetSection("ConnectionString").Value
        .Replace("CONNECTION_STRING_ID", CONNECTION_STRING_ID)
        .Replace("CONNECTION_STRING_PASSWORD", CONNECTION_STRING_PASSWORD))
);

builder.Services.AddTransient<TokenService>();

#pragma warning restore CS8602 // Desrefer�ncia de uma refer�ncia possivelmente nula.

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

app.MapControllers();

app.Run();
