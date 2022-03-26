using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;

namespace Nervestaple.WebApi.Policies
{
    /// <summary>
    /// Provides a requirement that the authorized identity be associated with
    /// specific roles
    /// </summary>
    public class HasRoleRequirement : IAuthorizationRequirement
    {
        /// <summary>
        /// List of required roles
        /// </summary>
        public List<string> Roles { get;  }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="role">Required role</param>
        public HasRoleRequirement(string role)
        {
            Roles = new List<string> {role};
        }

        /// <summary>
        /// Creates a new instance
        /// </summary>
        /// <param name="roles">List of required roles</param>
        public HasRoleRequirement(List<string> roles)
        {
            Roles = roles;
        }
    }
}