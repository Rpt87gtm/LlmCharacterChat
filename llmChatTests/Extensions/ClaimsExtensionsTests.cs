using api.Extensions;
using System.Security.Claims;
using Xunit;
using Assert = Xunit.Assert;

namespace apiTests.Extensions
{
    public class ClaimsExtensionsTests
    {
        [Fact]
        public void GetUsername_ShouldReturnUsername_WhenClaimExists()
        {
            // Arrange
            var claims = new[]
            {
                new Claim("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/givenname", "testuser")
            };
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            // Act
            var username = user.GetUsername();

            // Assert
            Assert.Equal("testuser", username);
        }

        [Fact]
        public void GetUsername_ShouldThrowInvalidOperationException_WhenClaimDoesNotExist()
        {
            // Arrange
            var claims = new Claim[] { }; // Нет нужного утверждения
            var identity = new ClaimsIdentity(claims);
            var user = new ClaimsPrincipal(identity);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => user.GetUsername());
            Assert.Equal("Claim not found", exception.Message);
        }

        [Fact]
        public void GetUsername_ShouldThrowInvalidOperationException_WhenUserIsNull()
        {
            // Arrange
            ClaimsPrincipal user = null;

            // Act & Assert
            var exception = Assert.Throws<ArgumentNullException>(() => user.GetUsername());
            Assert.Equal("user", exception.ParamName);
        }
    }
}