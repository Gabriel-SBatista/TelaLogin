using FluentValidation;
using Login.Core.Entities;
using Login.Core.Presenter;
using Login.Core.Repositories;
using Login.Core.Requests;
using Login.Core.Services.Hasher;
using Login.Core.Services.RabbitMQServices;
using Login.Core.Services.TokenService;
using Login.Core.Services.UserServices;
using Login.Core.Validators;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.UnitTests.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _repositoryMock = new();
        private readonly IValidator<UserRegisterRequest> _userRegisterValidator;
        private readonly IValidator<UserLoginRequest> _userLoginValidator;
        private readonly Mock<IEmailQueueService> _emailQueueServiceMock = new();
        private readonly Mock<ITokenService> _tokenServiceMock = new();
        private readonly Mock<IPasswordHasher> _passwordHasherMock = new();

        public UserServiceTests()
        {
            _userRegisterValidator = new UserRegisterValidator();
            _userLoginValidator = new UserLoginValidator();
        }

        private UserService GetService()
        {
            return new UserService(_repositoryMock.Object, _userRegisterValidator, _userLoginValidator, _emailQueueServiceMock.Object, _tokenServiceMock.Object, _passwordHasherMock.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_InvalidUserRequest_Returns_ErrorMessages()
        {
            var service = GetService();

            var userRegisterRequest = new UserRegisterRequest
            {
                Username = "",
                Email = "test.com",
                Password = "",
            };

            var result = await service.RegisterUserAsync(userRegisterRequest);

            Assert.False(result.Success);
            Assert.Contains("Insira um email valido", result.Messages);
            Assert.Contains("Insira o nome de usuario", result.Messages);
            Assert.Contains("Insira uma senha", result.Messages);
        }

        [Fact]
        public async Task RegisterUserAsync_EmailAlreadyExists_Returns_ErrorMessages()
        {
            var service = GetService();

            var userRegisterRequest = new UserRegisterRequest
            {
                Username = "UserTest",
                Email = "user@test.com",
                Password = "test123",
            };

            _repositoryMock.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User());

            var result = await service.RegisterUserAsync(userRegisterRequest);

            Assert.False(result.Success);
            Assert.Contains("Email ja cadastrado", result.Messages);
        }

        [Fact]
        public async Task RegisterUserAsync_UsernameAlreadyExists_Returns_ErrorMessages()
        {
            var service = GetService();

            var userRegisterRequest = new UserRegisterRequest
            {
                Username = "UserTest",
                Email = "user@test.com",
                Password = "test123",
            };

            _repositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User());

            var result = await service.RegisterUserAsync(userRegisterRequest);

            Assert.False(result.Success);
            Assert.Contains("Esse nome de usuario ja existe", result.Messages);
        }

        [Fact]
        public async Task RegisterUserAsync_ValidRequest_Returns_Success_And_UserCreated()
        {
            var service = GetService();

            _repositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User());
            _passwordHasherMock.Setup(x => x.HashPassowrd(It.Is<string>(x => x == "test123"))).Returns(new Password { Salt = "salt123", Hash = "hash321"});

            var userRegisterRequest = new UserRegisterRequest
            {
                Username = "UserTest",
                Email = "user@test.com",
                Password = "test123",
            };

            var result = await service.RegisterUserAsync(userRegisterRequest);

            _repositoryMock.Verify(
                x => x.CreateAsync(It.Is<User>(x =>
                    x.Username == "UserTest" &&
                    x.Email == "user@test.com" &&
                    x.PasswordHash == "hash321" &&
                    x.Salt == "salt123" &&
                    x.EmailConfirmed == false),
                It.IsAny<CancellationToken>()), Times.Once);

            Assert.True(result.Success);
            Assert.Null(result.Messages);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task LoginUserAsync_InvalidUserRequest_Returns_ErrorMessages()
        {
            var service = GetService();

            var userLoginRequest = new UserLoginRequest
            {
                Username = "",
                Password = ""
            };

            var result = await service.LoginUserAsync(userLoginRequest);

            Assert.False(result.Success);
            Assert.Contains("Insira um nome de usuario", result.Messages);
            Assert.Contains("Insira a senha", result.Messages);
        }

        [Fact]
        public async Task LoginUserAsync_InvalidUsername_Returns_ErrorMessage()
        {
            var service = GetService();

            var userLoginRequest = new UserLoginRequest
            {
                Username = "UserTest",
                Password = "test123"
            };

            var result = await service.LoginUserAsync(userLoginRequest);

            Assert.False(result.Success);
            Assert.Contains("Nome de usuario ou senha invalidos", result.Messages);
        }

        [Fact]
        public async Task LoginUserAsync_InvalidPassword_Returns_ErrorMessage()
        {
            var service = GetService();

            var userLoginRequest = new UserLoginRequest
            {
                Username = "UserTest",
                Password = "test123",
            };

            _repositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User());
            _passwordHasherMock.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = await service.LoginUserAsync(userLoginRequest);

            Assert.False(result.Success);
            Assert.Contains("Nome de usuario ou senha invalidos", result.Messages);
        }

        [Fact]
        public async Task LoginUserAsync_NotConfirmedEmail_Returns_ErrorMessage()
        {
            var service = GetService();

            var userLoginRequest = new UserLoginRequest
            {
                Username = "UserTest",
                Password = "test123",
            };

            _repositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User { EmailConfirmed = false });
            _passwordHasherMock.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = await service.LoginUserAsync(userLoginRequest);

            Assert.False(result.Success);
            Assert.Contains("Email não confirmado", result.Messages);
        }

        [Fact]
        public async Task LoginUserAsync_ValidRequest_Returns_Success_And_TokenInfo()
        {
            var service = GetService();

            var userLoginRequest = new UserLoginRequest
            {
                Username = "UserTest",
                Password = "test123"
            };

            _repositoryMock.Setup(x => x.GetByUsernameAsync(It.IsAny<string>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User { EmailConfirmed = true });
            _passwordHasherMock.Setup(x => x.VerifyPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            var result = await service.LoginUserAsync(userLoginRequest);

            Assert.True(result.Success);
            Assert.Null(result.Messages);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task ConfirmEmailAsync_UserNotFound_Returns_ErrorMessage()
        {
            var service = GetService();

            var result = await service.ConfirmEmailAsync(1);

            Assert.False(result.Success);
            Assert.Contains("Usuario não encontrado", result.Messages);
        }

        [Fact]
        public async Task ConfirmEmailAsync_EmailAlreadyConfirmed_Returns_ErrorMessage()
        {
            var service = GetService();

            _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User { EmailConfirmed = true });

            var result = await service.ConfirmEmailAsync(1);

            Assert.False(result.Success);
            Assert.Contains("Email ja confirmado", result.Messages);
        }

        [Fact]
        public async Task ConfirmEmailAsync_Returns_UserConfirmed()
        {
            var service = GetService();

            _repositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(new User { EmailConfirmed = false });

            var result = await service.ConfirmEmailAsync(1);

            _repositoryMock.Verify(x => x.UpdateAsync(It.Is<User>(x => x.EmailConfirmed == true), It.IsAny<CancellationToken>()), Times.Once());

            Assert.True(result.Success);
            Assert.Null(result.Messages);
            Assert.NotNull(result.Data);
        }

        [Fact]
        public async Task GetUserAsync_Returns_Users()
        {
            var service = GetService();

            var user1 = new User
            {
                Username = "UserTest1",
                Email = "user@test.com",
                PasswordHash = "testhash",
                Salt = "testsalt",
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            var user2 = new User
            {
                Username = "UserTest2",
                Email = "user2@test.com",
                PasswordHash = "testhash2",
                Salt = "testsalt2",
                EmailConfirmed = true,
                CreatedAt = DateTime.Now
            };

            _repositoryMock.Setup(x => x.GetAllAsync(It.IsAny<CancellationToken>())).ReturnsAsync(new List<User> { user1, user2 });

            var result = await service.GetUsersAsync();

            Assert.Equal(2, result.Data.Count);
            Assert.Equal("UserTest1", result.Data[0].Username);
            Assert.Equal("UserTest2", result.Data[1].Username);
        }
    }
}
