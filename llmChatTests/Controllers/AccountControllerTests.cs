using api.Controllers;
using api.Dtos.Account;
using api.Interfaces;
using api.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Query;
using Moq;
using System.Linq.Expressions;
using Xunit;
using Assert = Xunit.Assert;

namespace api.Tests.Controllers
{
    public class AccountControllerTests
    {
        private readonly Mock<UserManager<AppUser>> _mockUserManager;
        private readonly Mock<ITokenService> _mockTokenService;
        private readonly Mock<SignInManager<AppUser>> _mockSignInManager;
        private readonly AccountController _accountController;

        public AccountControllerTests()
        {
            var store = new Mock<IUserStore<AppUser>>();
            _mockUserManager = new Mock<UserManager<AppUser>>(store.Object, null, null, null, null, null, null, null, null);
            _mockTokenService = new Mock<ITokenService>();
            _mockSignInManager = new Mock<SignInManager<AppUser>>(_mockUserManager.Object,
                new Mock<IHttpContextAccessor>().Object,
                new Mock<IUserClaimsPrincipalFactory<AppUser>>().Object,
                null, null, null, null);

            _accountController = new AccountController(_mockUserManager.Object, _mockTokenService.Object, _mockSignInManager.Object);
        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserIsRegistered()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Password123!"
            };

            var appUser = new AppUser
            {
                UserName = registerDto.UserName,
                Email = registerDto.Email
            };

            _mockUserManager.Setup(manager => manager.CreateAsync(It.IsAny<AppUser>(), registerDto.Password))
                .ReturnsAsync(IdentityResult.Success);

            _mockUserManager.Setup(manager => manager.AddToRoleAsync(It.IsAny<AppUser>(), "User"))
                .ReturnsAsync(IdentityResult.Success);

            _mockTokenService.Setup(service => service.CreateToken(It.IsAny<AppUser>()))
                .Returns("fakeToken");

            // Act
            var result = await _accountController.Register(registerDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var newUserDto = Assert.IsType<NewUserDto>(okResult.Value);
            Assert.Equal(registerDto.UserName, newUserDto.UserName);
            Assert.Equal(registerDto.Email, newUserDto.Email);
            Assert.Equal("fakeToken", newUserDto.Token);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "",
                Email = "invalid-email",
                Password = "short"
            };

            _accountController.ModelState.AddModelError("UserName", "UserName is required");

            // Act
            var result = await _accountController.Register(registerDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.IsType<SerializableError>(badRequestResult.Value);
        }

        [Fact]
        public async Task Register_ShouldReturnStatusCode500_WhenUserCreationFails()
        {
            // Arrange
            var registerDto = new RegisterDto
            {
                UserName = "testuser",
                Email = "test@example.com",
                Password = "Password123!"
            };

            _mockUserManager.Setup(manager => manager.CreateAsync(It.IsAny<AppUser>(), registerDto.Password))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "User creation failed" }));

            // Act
            var result = await _accountController.Register(registerDto);

            // Assert
            var statusCodeResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, statusCodeResult.StatusCode);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "testuser",
                Password = "Password123!"
            };

            var appUser = new AppUser
            {
                UserName = loginDto.UserName,
                Email = "test@example.com"
            };

            var users = new List<AppUser> { appUser }.AsQueryable();

            var mockDbSet = new Mock<IQueryable<AppUser>>();
            mockDbSet.As<IAsyncEnumerable<AppUser>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<AppUser>(users.GetEnumerator()));

            mockDbSet.As<IQueryable<AppUser>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<AppUser>(users.Provider));

            mockDbSet.As<IQueryable<AppUser>>()
                .Setup(m => m.Expression)
                .Returns(users.Expression);

            mockDbSet.As<IQueryable<AppUser>>()
                .Setup(m => m.ElementType)
                .Returns(users.ElementType);

            mockDbSet.As<IQueryable<AppUser>>()
                .Setup(m => m.GetEnumerator())
                .Returns(users.GetEnumerator());

            _mockUserManager.Setup(manager => manager.Users)
                .Returns(mockDbSet.Object);

            _mockSignInManager.Setup(manager => manager.CheckPasswordSignInAsync(appUser, loginDto.Password, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Success);

            _mockTokenService.Setup(service => service.CreateToken(It.IsAny<AppUser>()))
                .Returns("fakeToken");

            // Act
            var result = await _accountController.Login(loginDto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var newUserDto = Assert.IsType<NewUserDto>(okResult.Value);
            Assert.Equal(loginDto.UserName, newUserDto.UserName);
            Assert.Equal("fakeToken", newUserDto.Token);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenUserDoesNotExist()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "nonexistentuser",
                Password = "Password123!"
            };

            var users = new List<AppUser>().AsQueryable();

            var mockDbSet = new Mock<IQueryable<AppUser>>();
            mockDbSet.As<IAsyncEnumerable<AppUser>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<AppUser>(users.GetEnumerator()));

            mockDbSet.As<IQueryable<AppUser>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<AppUser>(users.Provider));

            mockDbSet.As<IQueryable<AppUser>>()
                .Setup(m => m.Expression)
                .Returns(users.Expression);

            mockDbSet.As<IQueryable<AppUser>>()
                .Setup(m => m.ElementType)
                .Returns(users.ElementType);

            mockDbSet.As<IQueryable<AppUser>>()
                .Setup(m => m.GetEnumerator())
                .Returns(users.GetEnumerator());

            _mockUserManager.Setup(manager => manager.Users)
                .Returns(mockDbSet.Object);

            // Act
            var result = await _accountController.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Invalid username!", unauthorizedResult.Value);
        }

        [Fact]
        public async Task Login_ShouldReturnUnauthorized_WhenPasswordIsIncorrect()
        {
            // Arrange
            var loginDto = new LoginDto
            {
                UserName = "testuser",
                Password = "WrongPassword"
            };

            var appUser = new AppUser
            {
                UserName = loginDto.UserName,
                Email = "test@example.com"
            };

            var users = new List<AppUser> { appUser }.AsQueryable();

            var mockDbSet = new Mock<IQueryable<AppUser>>();
            mockDbSet.As<IAsyncEnumerable<AppUser>>()
                .Setup(m => m.GetAsyncEnumerator(It.IsAny<CancellationToken>()))
                .Returns(new TestAsyncEnumerator<AppUser>(users.GetEnumerator()));

            mockDbSet.As<IQueryable<AppUser>>()
                .Setup(m => m.Provider)
                .Returns(new TestAsyncQueryProvider<AppUser>(users.Provider));

            mockDbSet.As<IQueryable<AppUser>>()
                .Setup(m => m.Expression)
                .Returns(users.Expression);

            mockDbSet.As<IQueryable<AppUser>>()
                .Setup(m => m.ElementType)
                .Returns(users.ElementType);

            mockDbSet.As<IQueryable<AppUser>>()
                .Setup(m => m.GetEnumerator())
                .Returns(users.GetEnumerator());

            _mockUserManager.Setup(manager => manager.Users)
                .Returns(mockDbSet.Object);

            _mockSignInManager.Setup(manager => manager.CheckPasswordSignInAsync(appUser, loginDto.Password, false))
                .ReturnsAsync(Microsoft.AspNetCore.Identity.SignInResult.Failed);

            // Act
            var result = await _accountController.Login(loginDto);

            // Assert
            var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal("Username not found and/or password incorrect", unauthorizedResult.Value);
        }
    }

    // Вспомогательные классы для поддержки асинхронных операций
    internal class TestAsyncEnumerator<T> : IAsyncEnumerator<T>
    {
        private readonly IEnumerator<T> _inner;

        public TestAsyncEnumerator(IEnumerator<T> inner)
        {
            _inner = inner;
        }

        public T Current => _inner.Current;

        public ValueTask DisposeAsync()
        {
            _inner.Dispose();
            return ValueTask.CompletedTask;
        }

        public ValueTask<bool> MoveNextAsync()
        {
            return ValueTask.FromResult(_inner.MoveNext());
        }
    }

    internal class TestAsyncQueryProvider<T> : IAsyncQueryProvider
    {
        private readonly IQueryProvider _inner;

        internal TestAsyncQueryProvider(IQueryProvider inner)
        {
            _inner = inner;
        }

        public IQueryable CreateQuery(Expression expression)
        {
            return new TestAsyncEnumerable<T>(expression);
        }

        public IQueryable<TElement> CreateQuery<TElement>(Expression expression)
        {
            return new TestAsyncEnumerable<TElement>(expression);
        }

        public object Execute(Expression expression)
        {
            return _inner.Execute(expression);
        }

        public TResult Execute<TResult>(Expression expression)
        {
            return _inner.Execute<TResult>(expression);
        }

        public TResult ExecuteAsync<TResult>(Expression expression, CancellationToken cancellationToken = default)
        {
            var resultType = typeof(TResult).GetGenericArguments()[0];
            var executionResult = typeof(IQueryProvider)
                .GetMethod(nameof(Execute), 1, new[] { typeof(Expression) })
                .MakeGenericMethod(resultType)
                .Invoke(this, new[] { expression });

            return (TResult)typeof(Task).GetMethod(nameof(Task.FromResult))
                .MakeGenericMethod(resultType)
                .Invoke(null, new[] { executionResult });
        }
    }

    internal class TestAsyncEnumerable<T> : EnumerableQuery<T>, IAsyncEnumerable<T>, IQueryable<T>
    {
        public TestAsyncEnumerable(IEnumerable<T> enumerable)
            : base(enumerable)
        { }

        public TestAsyncEnumerable(Expression expression)
            : base(expression)
        { }

        public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        {
            return new TestAsyncEnumerator<T>(this.AsEnumerable().GetEnumerator());
        }
    }
}