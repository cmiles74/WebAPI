using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Nervestaple.WebApi.Helpers {
    /// <summary>
    /// Provides helper methods to make it easier to configure your application
    /// to provide Swagger interactive documentation for your REST-ful API.
    ///
    /// Swashbuckle is used to generate this documentation.
    /// </summary>
    public class SwaggerStartupHelper {
        /// <summary>
        /// Configures your application to produce Swaggger interactive
        /// documentation for your REST-ful API
        ///
        /// The XML documentation is produced by the build process, the path to
        /// the XML documentation should be something like...
        ///
        ///      AppContext.BaseDirectory, "YOUR_PROJECT_NAME.xml")
        /// 
        /// </summary>
        /// <param name="services">Collection of existing services</param>
        /// <param name="documentationPath">Path to the directory with your XML
        /// documentation</param>
        public static void ConfigureServices(
            IServiceCollection services,
            string documentationPath) {
            services.AddSwaggerGen(options => {
                options.OperationFilter<SwaggerVersionedApiDefaultValues>();
                
                // if we have a file, get it's parent directory
                var path = documentationPath;
                var attributes = File.GetAttributes(path);
                if (!attributes.HasFlag(FileAttributes.Directory)) {
                    path = Directory.GetParent(path).FullName;
                }
                
                // include all XML documentation files in the directory
                if (!string.IsNullOrEmpty(path))
                {
                    foreach (var name in Directory.GetFiles(path, 
                        "*.XML", SearchOption.AllDirectories))
                    {
                        options.IncludeXmlComments(filePath: name);
                    }
                }
            });
            services.AddSwaggerGenNewtonsoftSupport();
        }

        /// <summary>
        /// Configures the application builder to use Swagger to generate
        /// REST-ful API documentation.
        /// </summary>
        /// <param name="app">Application builder instance</param>
        /// <param name="provider">API version description provider</param>
        /// <param name="deploymentPath">Deployment path of the application</param>
        public static void ConfigureApplication(
            IApplicationBuilder app,
            IApiVersionDescriptionProvider provider,
            string deploymentPath) {
            if (deploymentPath == null) {
                
                app.UseSwagger();

                if (provider != null) {
                    app.UseSwaggerUI(
                        options => {
                            foreach (var description in provider.ApiVersionDescriptions) {
                                options.SwaggerEndpoint(
                                    $"/swagger/{description.GroupName}/swagger.json",
                                    description.GroupName.ToUpperInvariant());
                                options.DocExpansion(DocExpansion.None);
                            }
                        });
                }
            } else {
                
                // set the path for Swagger
                app.UseSwagger(config => {
                    config.PreSerializeFilters.Add((document, request) =>
                    {
                        document.Servers = new List<OpenApiServer> {
                            new OpenApiServer {
                                Url = $"/{deploymentPath}"
                            }
                        };
                    });
                });
                
                // set the path for Swagger UI
                app.UseSwaggerUI(
                    options => {
                        foreach ( var description in provider.ApiVersionDescriptions ) {
                            options.SwaggerEndpoint(
                                $"{description.GroupName}/swagger.json", 
                                description.GroupName.ToUpperInvariant());
                            options.DocExpansion(DocExpansion.None);
                        }
                    } );
            }
        }
    }
}