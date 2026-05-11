using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Intent.RoslynWeaver.Attributes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using TransactionsDisputePortal.Api.Services;
using TransactionsDisputePortal.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.Identity.ApplicationSecurityConfiguration", Version = "1.0")]

namespace TransactionsDisputePortal.Api.Configuration
{
    public static class ApplicationSecurityConfiguration
    {
        public static IServiceCollection ConfigureApplicationSecurity(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            services.AddHttpContextAccessor();

            // Configure JWT Bearer authentication
            var jwtSettings = configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            
            if (string.IsNullOrEmpty(secretKey))
            {
                throw new InvalidOperationException("JWT SecretKey is not configured in appsettings.json");
            }

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew = TimeSpan.Zero // Remove default 5 minute clock skew
                    };

                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false; // Set to true in production
                });

            services.AddAuthorization(ConfigureAuthorization);

            return services;
        }

        [IntentManaged(Mode.Ignore)]
        private static void ConfigureAuthorization(AuthorizationOptions options)
        {
            // Configure policies and other authorization options here. For example:
            // options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("role", "employee"));
            // options.AddPolicy("AdminOnly", policy => policy.RequireClaim("role", "admin"));
        }
    }
}