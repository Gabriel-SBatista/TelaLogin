using FluentValidation;
using Login.Core.Entities;
using Login.Core.Presenter;
using Login.Core.Repositories;
using Login.Core.Requests;
using Login.Core.Services.EmailServices;
using Login.Core.Services.TokenService;
using Login.Core.Utils;
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
        private readonly IEmailService _emailService;
        private readonly ITokenService _tokenService;

        public UserService(IUserRepository userRepository, IValidator<UserRegisterRequest> userRegisterValidator, IValidator<UserLoginRequest> userLoginValidator, 
            IEmailService emailService, ITokenService tokenService)
        {
            _userRepository = userRepository;
            _userRegisterValidator = userRegisterValidator;
            _userLoginValidator = userLoginValidator;
            _emailService = emailService;
            _tokenService = tokenService;
        }

        public async Task<DefaultResult<User>> RegisterUserAsync(UserRegisterRequest userRequest, CancellationToken cancellationToken = default)
        {
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

            var (hash, salt) = PasswordHasher.HashPassowrd(userRequest.Password);

            var user = new User
            {
                Username = userRequest.Username,
                Email = userRequest.Email,
                PasswordHash = hash,
                Salt = salt,
                EmailConfirmed = false,
                CreatedAt = DateTime.UtcNow
            };

            var userCreated = await _userRepository.CreateAsync(user, cancellationToken);

            var email = _emailService.WriteEmail(user.Id);
            await _emailService.SendEmailAsync(user.Email, email, cancellationToken);

            return new DefaultResult<User>(userCreated);
        }

        public async Task<DefaultResult<TokenInfo>> LoginUserAsync(UserLoginRequest userLoginRequest, CancellationToken cancellationToken)
        {
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

            bool validPassword = PasswordHasher.VerifyPassword(userLoginRequest.Password, user.PasswordHash, user.Salt);

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

            return new DefaultResult<TokenInfo>(tokenInfo);
        }

        public async Task<DefaultResult<User>> ConfirmEmailAsync(int userId, CancellationToken cancellationToken = default)
        {
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

            await _userRepository.UpdateAsync(user, cancellationToken);

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
