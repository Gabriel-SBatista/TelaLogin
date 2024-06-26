using FluentValidation;
using Login.Core.Entities;
using Login.Core.Presenter;
using Login.Core.Repositories;
using Login.Core.Requests;
using Login.Core.Services.Hasher;
using Login.Core.Services.RabbitMQServices;
using Login.Core.Services.TokenService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Core.Services.UserServices
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IValidator<UserRegisterRequest> _userRegisterValidator;
        private readonly IValidator<UserLoginRequest> _userLoginValidator;
        private readonly IEmailQueueService _emailQueueService;
        private readonly ITokenService _tokenService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger<UserService> _logger;
        private readonly IConfiguration _configuration;

        public UserService(IUserRepository userRepository, IValidator<UserRegisterRequest> userRegisterValidator, IValidator<UserLoginRequest> userLoginValidator, 
            IEmailQueueService emailQueueService, ITokenService tokenService, IPasswordHasher passwordHasher, ILogger<UserService> logger, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _userRegisterValidator = userRegisterValidator;
            _userLoginValidator = userLoginValidator;
            _emailQueueService = emailQueueService;
            _tokenService = tokenService;
            _passwordHasher = passwordHasher;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<DefaultResult<User>> RegisterUserAsync(UserRegisterRequest userRequest, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("starting user {username} registration", userRequest.Username);
            var result = _userRegisterValidator.Validate(userRequest);

            if (!result.IsValid)
            {
                return new DefaultResult<User>(result.Errors);
            }

            var userByEmail = await _userRepository.GetByEmailAsync(userRequest.Email, cancellationToken);

            if (userByEmail != null)
            {
                return new DefaultResult<User>("Email ja cadastrado");
            }

            var userByUsername = await _userRepository.GetByUsernameAsync(userRequest.Username, cancellationToken);

            if (userByUsername != null)
            {
                return new DefaultResult<User>("Esse nome de usuario ja existe");
            }

            _logger.LogInformation("starting password hash");
            var hashedPassword = _passwordHasher.HashPassowrd(userRequest.Password);
            _logger.LogInformation("password hash completed");

            var user = new User
            {
                Username = userRequest.Username,
                Email = userRequest.Email,
                PasswordHash = hashedPassword.Hash,
                Salt = hashedPassword.Salt,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            _logger.LogInformation("creating user {username} in database", user.Username);
            var userCreated = await _userRepository.CreateAsync(user, cancellationToken);
            _logger.LogInformation("user {username} successfully created", user.Username);

            var link = _configuration.GetSection("ConfirmationLink").Value;
            var email = new Email
            {
                Subject = "Email de confirmação",
                Body = $"Confirme sua conta acessando o link: {link}{userCreated.Id}",
                To = userCreated.Email
            };

            _emailQueueService.EnqueueEmail(email);

            _logger.LogInformation("user {username} successfully registered", user.Username);
            return new DefaultResult<User>(userCreated);
        }

        public async Task<DefaultResult<TokenInfo>> LoginUserAsync(UserLoginRequest userLoginRequest, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("starting user {username} login", userLoginRequest.Username);
            var result = _userLoginValidator.Validate(userLoginRequest);

            if (!result.IsValid)
            {
                return new DefaultResult<TokenInfo>(result.Errors);
            }

            var user = await _userRepository.GetByUsernameAsync(userLoginRequest.Username, cancellationToken);

            if (user == null)
            {
                return new DefaultResult<TokenInfo>("Nome de usuario ou senha invalidos");
            }

            bool validPassword = _passwordHasher.VerifyPassword(userLoginRequest.Password, user.PasswordHash, user.Salt);

            if (!validPassword)
            {
                return new DefaultResult<TokenInfo>("Nome de usuario ou senha invalidos");
            }

            if(user.EmailConfirmed == false)
            {
                return new DefaultResult<TokenInfo>("Email não confirmado");
            }

            var tokenInfo = new TokenInfo
            {
                Token = _tokenService.GenerateToken(user),
                UserId = user.Id,
                Username = user.Username
            };

            _logger.LogInformation("user {username} successfully login", user.Username);
            return new DefaultResult<TokenInfo>(tokenInfo);
        }

        public async Task<DefaultResult<User>> ConfirmEmailAsync(int userId, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("starting email confirmation for user {userId}", userId);
            var user = await _userRepository.GetByIdAsync(userId, cancellationToken);

            if (user == null)
            {
                return new DefaultResult<User>("Usuario não encontrado");
            }

            if (user.EmailConfirmed == true)
            {
                return new DefaultResult<User>("Email ja confirmado");
            }

            user.EmailConfirmed = true;

            _logger.LogInformation("updating user {username} in database", user.Username);
            await _userRepository.UpdateAsync(user, cancellationToken);

            _logger.LogInformation("user {username} confirmed successfully", user.Username);
            return new DefaultResult<User>(user);
        }

        public async Task<DefaultResult<List<UserPresenter>>> GetUsersAsync(CancellationToken cancellationToken = default)
        {
            var users = await _userRepository.GetAllAsync(cancellationToken);

            var usersPresenter = new List<UserPresenter>();

            foreach (var user in users)
            {
                usersPresenter.Add(new UserPresenter { Username = user.Username, Email = user.Email, CreatedAt = user.CreatedAt });
            }

            return new DefaultResult<List<UserPresenter>>(usersPresenter);
        }
    }
}
