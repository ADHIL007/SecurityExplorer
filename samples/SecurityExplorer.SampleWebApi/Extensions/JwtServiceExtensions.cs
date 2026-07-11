using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace SecurityExplorer.SampleWebApi.Extensions
{

    public static class JwtServiceExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtSettingSection = configuration.GetSection("JwtSettings");
            var secretkey = jwtSettingSection["SecretKey"];

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }
                ).AddJwtBearer(options =>
                {

                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettingSection["Issuer"],

                        ValidAudience = jwtSettingSection["Audience"],
                        IssuerSigningKey =
                    new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretkey!)),
                        ClockSkew = TimeSpan.Zero,
                    };
                }


                );

            return services;

        }

    }

}


