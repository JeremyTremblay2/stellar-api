using Microsoft.IdentityModel.Tokens;
using StellarApi.Model.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace StellarApi.RestApi.Auth
{
    /// <summary>
    /// Service class responsible for creating and managing JWT tokens.
    /// </summary>
    public class TokenService : ITokenService
    {
        /// <summary>
        /// The expiration time of the token, in minutes.
        /// </summary>
        private const int ExpirationMinutes = 30;

        /// <summary>
        /// A logger used to store API calls.
        /// </summary>
        private readonly ILogger<TokenService> _logger;

        /// <summary>
        /// Represents the configuration settings for the application.
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="TokenService"/> class.
        /// </summary>
        /// <param name="logger">The logger instance.</param>
        /// <param name="configuration">The configuration settings for the application.</param>
        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <inheritdoc/>
        public string GenerateAccessToken(User user)
        {
            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
            var token = CreateJwtToken(
                CreateClaims(user),
                CreateSigningCredentials(),
                expiration
            );
            var tokenHandler = new JwtSecurityTokenHandler();

            _logger.LogTrace($"JWT Token created for user: {user.Id}");

            return tokenHandler.WriteToken(token);
        }

        /// <inheritdoc/>
        public string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        /// <inheritdoc/>
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;

            var symmetricSecurityKey = _configuration.GetValue<string>("JwtTokenSettings:SymmetricSecurityKey");
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ClockSkew = TimeSpan.Zero,
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false,
                ValidateIssuerSigningKey = true,
                ValidIssuer = _configuration.GetValue<string>("JwtTokenSettings:ValidIssuer"),
                ValidAudience = _configuration.GetValue<string>("JwtTokenSettings:ValidAudience"),
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(symmetricSecurityKey)
                ),
            }, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }
            return principal;
        }

        /// <summary>
        /// Creates a JWT token with the specified claims, signing credentials, and expiration time.
        /// </summary>
        /// <param name="claims">The claims to include in the token.</param>
        /// <param name="credentials">The signing credentials used to sign the token.</param>
        /// <param name="expiration">The expiration time of the token.</param>
        /// <returns>The created JWT token.</returns>
        private JwtSecurityToken CreateJwtToken(List<Claim> claims, SigningCredentials credentials, DateTime expiration)
            => new(
                _configuration.GetSection("JwtTokenSettings")["ValidIssuer"],
                _configuration.GetSection("JwtTokenSettings")["ValidAudience"],
                claims,
                expires: expiration,
                signingCredentials: credentials
            );

        /// <summary>
        /// Creates the claims for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the claims are created.</param>
        /// <returns>The list of claims for the user.</returns>
        private List<Claim> CreateClaims(User user)
        {
            var jwtSub = _configuration.GetSection("JwtTokenSettings")["JwtRegisteredClaimNamesSub"];

            try
            {
                var claims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Sub, jwtSub),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()),
                        new Claim(ClaimTypes.Name, user.Username),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Role, user.Role.ToString())
                    };

                return claims;
            }
            catch (Exception e)
            {
                _logger.LogError($"Error creating claims for user: {user.Id}. {e.Message}");
                throw;
            }
        }

        /// <summary>
        /// Creates the signing credentials for the JWT token.
        /// </summary>
        /// <returns>The signing credentials.</returns>
        private SigningCredentials CreateSigningCredentials()
        {
            var symmetricSecurityKey = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetSection("JwtTokenSettings")["SymmetricSecurityKey"];

            return new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(symmetricSecurityKey)
                ),
                SecurityAlgorithms.HmacSha256
            );
        }
    }
}