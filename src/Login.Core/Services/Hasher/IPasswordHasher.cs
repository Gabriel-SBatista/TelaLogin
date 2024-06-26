using Login.Core.Presenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Core.Services.Hasher
{
    public interface IPasswordHasher
    {
        Password HashPassowrd(string password);
        bool VerifyPassword(string password, string hash, string salt);
    }
}
