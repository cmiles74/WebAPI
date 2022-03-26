using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace Nervestaple.WebApi.Controllers
{
    /// <summary>
    /// Provides middleware that allows anonymous access to OPTIONS methods
    /// </summary>
    public class AnonymousOptionsMiddleware
    {
        private readonly RequestDelegate _next;

        /// <summary>
        /// Creates a new instances
        /// </summary>
        /// <param name="next">the next RequestDelegate in the chain</param>
        public AnonymousOptionsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        /// <summary>
        ///  Invokes this middleware with the provided HTTP context
        /// </summary>
        /// <param name="context">current HTTP context</param>
        public async Task Invoke(HttpContext context)
        {   
            // require authentication for everything but OPTIONS methods
            if (context.Request.Method != "OPTIONS")
            {
                if (context.User.Identity.IsAuthenticated)
                {
                    await _next(context);
                    return;
                } 
                
                // the current account isn't authenticated, force negotiation
                await context.ChallengeAsync("Windows");
            }
            
            await _next(context);
        }
    }
}