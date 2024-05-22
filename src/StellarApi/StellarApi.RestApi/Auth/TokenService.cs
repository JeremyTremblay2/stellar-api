using Microsoft.IdentityModel.Tokens;
using StellarApi.Model.Users;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
        private const int ExpirationMinutes = 50;

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
        public TokenService(IConfiguration configuration, ILogger<TokenService> logger)
        {
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Creates a JWT token for the specified user.
        /// </summary>
        /// <param name="user">The user for whom the token is created.</param>
        /// <returns>The generated JWT token.</returns>
        public string CreateToken(User user)
        {
            var expiration = DateTime.UtcNow.AddMinutes(ExpirationMinutes);
            var token = CreateJwtToken(
                CreateClaims(user),
                CreateSigningCredentials(),
                expiration
            );
            var tokenHandler = new JwtSecurityTokenHandler();

            _logger.LogInformation($"JWT Token created for user: {user}");

            return tokenHandler.WriteToken(token);
        }

        /// <summary>
        /// Gets the role of the specified user.
        /// </summary>
        /// <param name="user">The user for whom the role is retrieved.</param>
        /// <returns>The role of the user.</returns>
        public Role GetUserRole(User user)
        {
            var adminUsername = _configuration.GetSection("SiteSettings")["AdminEmail"];
            return user.Email == adminUsername ? Role.Administrator : Role.Member;
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
                _logger.LogError(e.Message);
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