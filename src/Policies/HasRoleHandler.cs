using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Nervestaple.WebApi.Policies
{
    /// <summary>
    /// Provides an authorization handler that requires an authorized identity
    /// be associated with one or more roles
    /// </summary>
    public class HasRoleHandler : AuthorizationHandler<HasRoleRequirement>
    {
        /// <inheritdoc />
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context, HasRoleRequirement requirement)
        {

            int roleCount = 0;
            foreach (var role in requirement.Roles) {
                if (context.User.HasClaim((Predicate<Claim>) (c =>
                    c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role" && c.Value == role)))
                    roleCount++;
            }

            if (roleCount == requirement.Roles.Count) {
                context.Succeed((IAuthorizationRequirement) requirement);
            }
            
            return Task.CompletedTask;
        }
    }
}