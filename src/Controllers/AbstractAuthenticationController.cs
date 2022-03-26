using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Nervestaple.WebApi.Models.security;
using Nervestaple.WebService.Models.security;
using Nervestaple.WebService.Services.security;

namespace Nervestaple.WebApi.Controllers
{
    /// <summary>
    /// Provides an endpoint for account authentication
    /// </summary>
    [ApiController]
    [Route("Authenticate")]
    public abstract class AbstractAuthenticationController : ControllerBase
    {
        /// <summary>
        /// Account service for accessing account data
        /// </summary>
        protected readonly IAccountService AccountService;
        
        /// <summary>
        /// Security configuration information
        /// </summary>
        protected readonly SecurityConfiguration SecurityConfiguration;

        /// <summary>
        /// Creates a new controller instance
        /// </summary>
        /// <param name="service">Backing service instance</param>
        /// <param name="securityConfiguration">Security configuration information</param>
        protected AbstractAuthenticationController(IAccountService service, SecurityConfiguration securityConfiguration)
        {
            SecurityConfiguration = securityConfiguration;
            AccountService = service;
        }
        
        /// <summary>
        /// Accepts a credentials object and uses those credentials to
        /// authenticate, if successful a token about the account associated
        /// with those credentials is returned
        /// </summary>
        /// <param name="credentials">Authentication credentials</param>
        /// <returns>Account resource</returns>
        /// <response code="404">The provided credentials are not associated with an account</response>
        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(typeof(AuthorizedAccount), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public virtual async Task<IActionResult> Post([FromBody] SimpleAccountCredentials credentials)
        {
            Account account = await AccountService.AuthenticateAsync(credentials);
            if (account != null)
            {
                // setup our identity and role claims
                List<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.Name, account.Id));
                foreach (var role in account.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }
                
                // create and sign our token
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(SecurityConfiguration.SigningKey);
                var descriptor = new SecurityTokenDescriptor
                {
                    
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(3),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(descriptor);
                
                return Ok(new AuthorizedAccount(account, tokenHandler.WriteToken(token)));
            }

            return NotFound();
        }
    }
}