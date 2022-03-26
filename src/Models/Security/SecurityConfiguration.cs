namespace Nervestaple.WebApi.Models.security
{
    /// <summary>
    /// Models all of the security configuration data
    /// </summary>
    public class SecurityConfiguration
    {
        /// <summary>
        /// Key used to sign security tokens.
        /// </summary>
        public string SigningKey { get; set; }
    }
}