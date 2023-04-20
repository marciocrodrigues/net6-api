using Blog.Data;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace Blog.Extensions
{
    public static class BuilderConfigurationExtension
    {
        private static string CONNECTION_STRING_ID = Environment.GetEnvironmentVariable("CONNECTION_STRING_SQL_SERVER_ID", EnvironmentVariableTarget.Machine);
        private static string CONNECTION_STRING_PASSWORD = Environment.GetEnvironmentVariable("CONNECTION_STRING_SQL_SERVER_PASSWORD", EnvironmentVariableTarget.Machine);

        public static WebApplicationBuilder LoadConfiguration(this WebApplicationBuilder builder)
        {
            Configuration.JwtKey = builder.Configuration.GetValue<string>("JwtKey");
            Configuration.ApiKeyName = builder.Configuration.GetValue<string>("ApiKeyName");
            Configuration.ApiKey = builder.Configuration.GetValue<string>("ApiKey");

            var smtp = new Configuration.SmtpConfiguration();
            builder.Configuration.GetSection("Smtp").Bind(smtp);
            Configuration.Smtp = smtp;

            return builder;
        }

        public static IServiceCollection LoadConfigSwagger(this IServiceCollection service)
        {
            service.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "APIContagem", Version = "v1" });

                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description =
                        "JWT Authorization Header - utilizado com Bearer Authentication.\r\n\r\n" +
                        "Digite 'Bearer' [espaço] e então seu token no campo abaixo.\r\n\r\n" +
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

            return service;
        }

        public static IServiceCollection LoadConfigAuthentication(this IServiceCollection service, byte[] key)
        {
            service.AddAuthentication(x =>
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

            return service;
        }

        public static IServiceCollection LoadConfigDataContext(this IServiceCollection service, ConfigurationManager configuration)
        {
            service.AddDbContext<BlogDataContext>(options =>
                options.UseSqlServer(configuration
                    .GetValue<string>("ConnectionString")
                    .Replace("CONNECTION_STRING_ID", CONNECTION_STRING_ID)
                    .Replace("CONNECTION_STRING_PASSWORD", CONNECTION_STRING_PASSWORD))
            );

            return service;
        }

        public static IServiceCollection LoadDependecyInjections(this IServiceCollection service)
        {
            service.AddTransient<TokenService>();
            service.AddTransient<EmailService>();
            return service;
        }
    }
}
