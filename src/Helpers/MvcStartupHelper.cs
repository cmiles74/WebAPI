using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Nervestaple.WebApi.Helpers {
    /// <summary>
    /// Provides helper methods to make it easier to configure your WebAPI
    /// project.
    /// </summary>
    public class MvcStartupHelper {
        /// <summary>
        /// Configures your application to use Newtonsoft's JSON library for
        /// exporting JSON data and, optionally, configures MVC to provide a
        /// versioned REST-ful API.
        /// </summary>
        /// <param name="services">Collection of existing services</param>
        /// <param name="defaultApiVersion">Default API version for your
        /// application</param>
        public static void ConfigureServices(
            IServiceCollection services,
            ApiVersion defaultApiVersion) {
            
            // we're still using the Newtonsoft JSON library
            services.AddMvc().AddNewtonsoftJson();

            services.AddVersionedApiExplorer(options => {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
            services.AddApiVersioning(options => {
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.DefaultApiVersion = defaultApiVersion;
            });
        }
    }
}