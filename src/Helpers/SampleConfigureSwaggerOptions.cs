using System;
using System.IO;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Nervestaple.WebApi.Helpers {
    /// <summary>
    /// Configures the Swagger generation options.
    /// </summary>
    /// <remarks>This allows API versioning to define a Swagger document per API version after the
    /// <see cref="IApiVersionDescriptionProvider"/> service has been resolved from the service container.</remarks>
    public class SampleConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        readonly IApiVersionDescriptionProvider _provider;

        /// <summary>
        /// Initializes a new instance of the ConfigureSwaggerOptions class in the Swashbuckle project.
        /// </summary>
        /// <param name="provider">The <see cref="IApiVersionDescriptionProvider">provider</see> used to generate Swagger documents.</param>
        public SampleConfigureSwaggerOptions( IApiVersionDescriptionProvider provider ) =>
            _provider = provider;

        /// <inheritdoc />
        public void Configure(SwaggerGenOptions options)
        {
            foreach ( var description in _provider.ApiVersionDescriptions ) {
                var info = new OpenApiInfo() {
                    Title = "My Awesome API",
                    Version = description.ApiVersion.ToString(),
                    Description = "API for accessing my awesome features",
                    License = new OpenApiLicense() {Name = "MIT", Url = new Uri("https://opensource.org/licenses/MIT")}
                };
               
                if ( description.IsDeprecated )
                {
                    info.Description += " This API version has been deprecated.";
                }
               
                options.SwaggerDoc(description.GroupName, info);
                options.EnableAnnotations();
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, "awesome-api.xml"), true);
            }
           
            // swagger authentication setup
            JwtStartupHelper.ConfigureSwagger(options);
        }
    }
}