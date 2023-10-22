using code_test_api.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace code_test_api.Services
{
    public class Utils
    {
        private readonly IConfiguration _config;

        public Utils(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Verify the hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        /// <returns></returns>
        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i]) return false;
                }
                return true;
            }
        }

        /// <summary>
        /// Create a password hash
        /// </summary>
        /// <param name="password"></param>
        /// <param name="passwordHash"></param>
        /// <param name="passwordSalt"></param>
        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        /// <summary>
        /// Gets the JWT for a given user object
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string? CreateJWT(User user)
        {
            try
            {
                // setup claims (username + email)
                var claims = new[] {
                new Claim(ClaimTypes.NameIdentifier, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
               
                };

                var key1 = _config.GetSection("JwtSettings:Key").Value;
                 // Get the private key from appsettings
                var key = new SymmetricSecurityKey(Encoding.UTF8
                    .GetBytes(_config.GetSection("JwtSettings:Key").Value));

                // setup signing creds
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

                // setup token descriptor
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.Now.AddHours(1),
                    SigningCredentials = creds, 
                    Issuer = _config.GetSection("JwtSettings:Issuer").Value, 
                    Audience = _config.GetSection("JwtSettings:Audience").Value
                };

                // Create the token
                var tokenHandler = new JwtSecurityTokenHandler();

                var token = tokenHandler.CreateToken(tokenDescriptor);

                var jwtToken = tokenHandler.WriteToken(token);

                return jwtToken;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}

