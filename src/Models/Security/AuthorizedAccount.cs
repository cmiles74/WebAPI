using Nervestaple.WebService.Models.security;

namespace Nervestaple.WebApi.Models.security
{
    /// <summary>
    /// Provides a model representing an authorized account
    /// </summary>
    public class AuthorizedAccount : Account
    {
        /// <summary>
        /// Creates a new authorized account and sets it's data with the data
        /// contained in the provided account instance and the provided
        /// authorization token
        /// </summary>
        /// <param name="account">Account information</param>
        /// <param name="token">Authorization token</param>
        public AuthorizedAccount(Account account, string token) : base(account)
        {
            Token = token;
        }

        /// <summary>
        /// Authorization token for the account
        /// </summary>
        public string Token { get; set; }
    }
}