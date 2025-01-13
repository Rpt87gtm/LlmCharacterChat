using api.Models.User;
using api.Service;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Xunit;
using Assert = Xunit.Assert;

namespace llmChatTests.Service
{
    public class TokenServiceTests
    {
        private readonly IConfiguration _config;
        private readonly TokenService _tokenService;

        public TokenServiceTests()
        {
            // Настройка мока IConfiguration
            var inMemorySettings = new Dictionary<string, string> {
                {"JWT:SigningKey", "WhoReadThisWillDie00000100fqefq23f2f4q4q4qe7r3892gf89gafqwef872f7g3f8yyg"},
                {"JWT:Issuer", "test_issuer"},
                {"JWT:Audience", "test_audience"}
            };

            _config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _tokenService = new TokenService(_config);
        }

        [Fact]
        public void CreateToken_ShouldReturnValidJwtToken()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user1",
                UserName = "testuser",
                Email = "testuser@example.com"
            };

            // Act
            var token = _tokenService.CreateToken(user);

            // Assert
            Assert.NotNull(token);
            Assert.NotEmpty(token);

            // Проверка содержимого токена
            var tokenHandler = new JwtSecurityTokenHandler();
            var jwtToken = tokenHandler.ReadJwtToken(token);

            Assert.Equal("test_issuer", jwtToken.Issuer);
            Assert.Equal("test_audience", jwtToken.Audiences.First());
            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.Name && c.Value == user.Email);
            Assert.Contains(jwtToken.Claims, c => c.Type == JwtRegisteredClaimNames.GivenName && c.Value == user.UserName);
            Assert.Contains(jwtToken.Claims, c => c.Type == "userId" && c.Value == user.Id);
        }

        [Fact]
        public void CreateToken_ShouldThrowArgumentNullException_WhenUserEmailIsNull()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user1",
                UserName = "testuser",
                Email = null // Email is null
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _tokenService.CreateToken(user));
            Assert.Equal("Email", exception.ParamName);
        }

        [Fact]
        public void CreateToken_ShouldThrowArgumentNullException_WhenUserNameIsNull()
        {
            // Arrange
            var user = new AppUser
            {
                Id = "user1",
                UserName = null, // UserName is null
                Email = "testuser@example.com"
            };

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => _tokenService.CreateToken(user));
            Assert.Equal("UserName", exception.ParamName);
        }

        [Fact]
        public void Constructor_ShouldThrowInvalidOperationException_WhenSigningKeyIsNotConfigured()
        {
            // Arrange
            var inMemorySettings = new Dictionary<string, string> {
                {"JWT:Issuer", "test_issuer"},
                {"JWT:Audience", "test_audience"}
                // SigningKey is missing
            };

            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => new TokenService(config));
            Assert.Equal("JWT SigningKey is not configured.", exception.Message);
        }
    }
}