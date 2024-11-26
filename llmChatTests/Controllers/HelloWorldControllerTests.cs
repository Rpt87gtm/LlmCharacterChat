using Xunit;
using Microsoft.AspNetCore.Mvc;
using Assert = Xunit.Assert;

namespace llmChat.Controllers.Tests
{
    public class HelloWorldControllerTests
    {
        [Fact()]
        public void GetTest()
        {
            // Arrange
            var controller = new HelloWorldController();

            // Act
            var result = controller.Get();

            // Assert
            var actionResult = Assert.IsType<ActionResult<string>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal("Hello, World! qwe", okResult.Value);
        }
    }
}