using Login.Core.Entities;
using Login.Core.Presenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Core.Services.TokenService
{
    public interface ITokenService
    {
        string GenerateToken(User user);
        TokenInfo? ValidateToken(string token);
    }
}
