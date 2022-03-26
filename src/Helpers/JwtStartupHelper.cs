using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Nervestaple.WebApi.Filter;
using Nervestaple.WebApi.Models.security;
using Nervestaple.WebService.Services.security;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nervestaple.WebApi.Helpers
{
    /// <summary>
    /// Provides helper methods to make it easier to configure your
    /// application for JWT token authentication.
    /// </summary>
    public class JwtStartupHelper
    {
        /// <summary>
        /// Configures JWT token authentication using the AccountService that is
        /// registered in your service collection. This method presumes that you
        /// have a section of your configuration called "Security" with the
        /// appropriate settings.
        /// </summary>
        /// <param name="configuration">Configuration instance</param>
        /// <param name="services">Application services</param>
        public static void ConfigureServicesForJwtBearerAuthentication(
            IConfiguration configuration, IServiceCollection services)
        {
            SecurityConfiguration securityConfiguration = new SecurityConfiguration();
            configuration.Bind("Security", securityConfiguration);
            services.AddSingleton(securityConfiguration);
            services.AddAuthentication(auth =>
            {
                auth.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                auth.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(bearer =>
            {
                bearer.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        var accountService =
                            context.HttpContext.RequestServices.GetRequiredService<IAccountService>();
                        var accountId = context.Principal.Identity.Name;
                        var account = accountService.Find(accountId);
                        if (account == null)
                        {
                            context.Fail("Unauthorized");
                        }

                        return Task.CompletedTask;
                    }
                };
                bearer.RequireHttpsMetadata = false;
                bearer.SaveToken = true;
                bearer.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(securityConfiguration.SigningKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });
        }

        /// <summary>
        /// Configures Swagger for JWT authentication; this method adds the
        /// required security definition and  instructs the customer to paste
        /// in their JWT token when testing API methods.
        /// </summary>
        /// <param name="options">Swagger generator options</param>
        public static void ConfigureSwagger(SwaggerGenOptions options)
        {
            options.OperationFilter<AuthorizeCheckOperationFilter>();
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT authorization header using the bearer scheme, example:" +
                              " \"Authorization: Bearer {token}\". Don't forget to type in" +
                              " \"Bearer\" before your token when you fill in the field!" ,
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey
            });
        }
    }
}