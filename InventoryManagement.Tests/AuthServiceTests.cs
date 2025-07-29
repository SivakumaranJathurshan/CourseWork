using InventoryManagement.Data.Repositories.Interfaces;
using InventoryManagement.Models;
using InventoryManagement.Models.DTO;
using InventoryManagement.Services;
using InventoryManagement.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryManagement.Tests
{
    public class AuthServiceTests
    {
        private readonly IUserRepository _repo;
        private readonly IPasswordHasher<User> _hasher;
        private readonly IConfiguration _config;

        private readonly IAuthService _service;

        public AuthServiceTests()
        {
            _repo = Substitute.For<IUserRepository>();
            _hasher = Substitute.For<IPasswordHasher<User>>();

            var inMemorySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "12345678901234567890123456789012" },
                { "Jwt:Issuer", "test_issuer" },
                { "Jwt:Audience", "test_audience" },
                { "Jwt:ExpireMinutes", "30" }
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _service = new AuthService(_repo, _hasher, _config);
        }

        #region Register Tests

        [Fact]
        public void Register_ShouldFail_WhenPasswordsDoNotMatch()
        {
            var dto = new RegisterDTO("Test", "User", "test@example.com", "Pass123", "WrongPass");

            var result = _service.Register(dto);

            Assert.False(result.Success);
            Assert.Equal("PPassword not matched", result.Error);
        }

        [Fact]
        public void Register_ShouldFail_WhenUserAlreadyExists()
        {
            var dto = new RegisterDTO("Test", "User", "existing@example.com", "Pass123", "Pass123");

            _repo.GetByEmail(dto.Email).Returns(new User());

            var result = _service.Register(dto);

            Assert.False(result.Success);
            Assert.Equal("User already exists", result.Error);
        }

        [Fact]
        public void Register_ShouldSucceed_WhenValid()
        {
            var dto = new RegisterDTO("Test", "User", "new@example.com", "Pass123", "Pass123");

            _repo.GetByEmail(dto.Email).Returns((User)null!);
            _hasher.HashPassword(Arg.Any<User>(), dto.Password).Returns("hashed");

            var result = _service.Register(dto);

            _repo.Received().Add(Arg.Is<User>(u => u.Email == dto.Email));
            Assert.True(result.Success);
            Assert.Null(result.Error);
        }

        #endregion

        #region Signin Tests

        [Fact]
        public void Signin_ShouldFail_WhenUserDoesNotExist()
        {
            var dto = new LoginDTO("missing@example.com", "Pass123");

            _repo.GetByEmail(dto.Email).Returns((User)null!);

            var result = _service.Signin(dto);

            Assert.False(result.Success);
            Assert.Equal("Invalid credentials", result.Error);
        }

        [Fact]
        public void Signin_ShouldFail_WhenPasswordInvalid()
        {
            var dto = new LoginDTO("user@example.com", "wrongpass");

            var user = new User { Email = dto.Email, PasswordHash = "hashed" };
            _repo.GetByEmail(dto.Email).Returns(user);
            _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password)
                   .Returns(PasswordVerificationResult.Failed);

            var result = _service.Signin(dto);

            Assert.False(result.Success);
            Assert.Equal("Invalid credentials", result.Error);
        }

        [Fact]
        public void Signin_ShouldSucceed_WhenCredentialsAreValid()
        {
            var dto = new LoginDTO("user@example.com", "correctpass");
            var user = new User
            {
                Id = 1,
                Email = dto.Email,
                FirstName = "Test",
                PasswordHash = "hashed"
            };

            _repo.GetByEmail(dto.Email).Returns(user);
            _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password)
                   .Returns(PasswordVerificationResult.Success);

            var result = _service.Signin(dto);

            Assert.True(result.Success);
            Assert.NotNull(result.Token);
            Assert.Null(result.Error);
        }

        [Fact]
        public void Signin_ShouldThrowException_WhenJwtKeyTooShort()
        {
            // Config with short key
            var shortKeySettings = new Dictionary<string, string>
            {
                { "Jwt:Key", "short" },
                { "Jwt:Issuer", "test_issuer" },
                { "Jwt:Audience", "test_audience" },
                { "Jwt:ExpireMinutes", "30" }
            };

            var badConfig = new ConfigurationBuilder().AddInMemoryCollection(shortKeySettings).Build();
            var badService = new AuthService(_repo, _hasher, badConfig);

            var dto = new LoginDTO("user@example.com", "correctpass");
            var user = new User { Id = 1, Email = dto.Email, FirstName = "Test", PasswordHash = "hashed" };

            _repo.GetByEmail(dto.Email).Returns(user);
            _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password)
                   .Returns(PasswordVerificationResult.Success);

            var ex = Assert.Throws<InvalidOperationException>(() => badService.Signin(dto));
            Assert.Equal("JWT Key must be at least 256 bits (32 characters).", ex.Message);
        }

        #endregion
    }
}
