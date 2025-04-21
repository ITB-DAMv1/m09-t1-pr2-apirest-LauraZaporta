using System.IdentityModel.Tokens.Jwt;

namespace Client.Tools
{
    public static class TokenHelper
    {
        public static bool IsTokenSession(string token)
        {
            return !string.IsNullOrEmpty(token) && !IsTokenExpired(token);
        }
        /// <summary>
        /// Validar si el token ha expirat o no
        /// </summary>
        /// <param name="token">Token de sessió</param>
        /// <returns></returns>
        public static bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwt = handler.ReadJwtToken(token);
            var expiration = jwt.ValidTo;
            return expiration < DateTime.UtcNow;
        }
    }
}
