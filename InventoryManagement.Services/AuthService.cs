using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Models.DTO;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace InventoryManagement.Services
{
    /// <summary>
    /// Service responsible for handling user authentication and registration logic.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _repo;
        private readonly Microsoft.AspNetCore.Identity.IPasswordHasher<User> _hasher;
        private readonly IConfiguration _config;

        /// <summary>
        /// Initializes a new instance of the AuthService.
        /// </summary>
        /// <param name="repo">User repository to interact with user data store.</param>
        /// <param name="hasher">Password hasher for secure password hashing and verification.</param>
        /// <param name="config">Application configuration to access JWT settings.</param>
        public AuthService(IUserRepository repo, Microsoft.AspNetCore.Identity.IPasswordHasher<User> hasher, IConfiguration config)
        {
            _repo = repo;
            _hasher = hasher;
            _config = config;
        }

        /// <summary>
        /// Registers a new user if the email is not already taken and passwords match.
        /// </summary>
        /// <param name="request">DTO containing registration data.</param>
        /// <returns>A tuple indicating success or failure and an error message if failed.</returns>
        public (bool Success, string? Error) Register(RegisterDTO request)
        {
            if (request.Password != request.ConfirmPassword)
                return (false, "Password not matched");

            if (_repo.GetByEmail(request.Email) != null)
                return (false, "User already exists");

            var user = new User { Email = request.Email, FirstName = request.FirstName, LastName = request.LastName };
            user.PasswordHash = _hasher.HashPassword(user, request.Password);
            _repo.Add(user);

            return (true, null);
        }

        /// <summary>
        /// Authenticates the user and generates a JWT token if credentials are valid.
        /// </summary>
        /// <param name="request">DTO containing login credentials.</param>
        /// <returns>A tuple indicating success, a JWT token if successful, and an error message if failed.</returns>
        public (bool Success, string? Token, string? Error) Signin(LoginDTO request)
        {
            var user = _repo.GetByEmail(request.Email);
            if (user == null)
                return (false, null, "Invalid credentials");

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
            if (result == PasswordVerificationResult.Failed)
                return (false, null, "Invalid credentials");

            var token = GenerateJwtToken(user);
            return (true, token, null);
        }

        /// <summary>
        /// Generates a signed JWT token with claims based on user identity.
        /// </summary>
        /// <param name="user">The authenticated user object.</param>
        /// <returns>A JWT token string.</returns>
        private string GenerateJwtToken(User user)
        {
            var settings = _config.GetSection("Jwt");

            var secret = settings["Key"] ?? throw new InvalidOperationException("JWT Key is missing in configuration.");
            if (secret.Length < 16)
                throw new InvalidOperationException("JWT Key must be at least 256 bits (32 characters).");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.FirstName),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: settings["Issuer"],
                audience: settings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(int.TryParse(settings["ExpireMinutes"], out var minutes) ? minutes : 60),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
