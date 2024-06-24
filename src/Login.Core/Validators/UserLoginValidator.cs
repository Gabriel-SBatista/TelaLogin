using FluentValidation;
using Login.Core.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Core.Validators
{
    public class UserLoginValidator : AbstractValidator<UserLoginRequest>
    {
        public UserLoginValidator()
        {
            RuleFor(x => x.Username).NotEmpty()
                .WithMessage("Insira um nome de usuario");
            RuleFor(x => x.Password).NotEmpty()
                .WithMessage("Insira a senha");
        }
    }
}
