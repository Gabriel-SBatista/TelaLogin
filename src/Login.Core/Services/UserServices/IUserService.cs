using Login.Core.Entities;
using Login.Core.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Core.Services.UserServices
{
    public interface IUserService
    {
        Task<DefaultResult<User>> RegisterUserAsync(UserRegisterRequest userRequest, CancellationToken cancellationToken = default);
        Task<DefaultResult<TokenInfo>> LoginUserAsync(UserLoginRequest userLoginRequest, CancellationToken cancellationToken = default);
        Task<DefaultResult<User>> ConfirmEmailAsync(string token, CancellationToken cancellationToken = default);
    }
}
