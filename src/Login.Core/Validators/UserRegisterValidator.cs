using FluentValidation;
using Login.Core.Requests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Core.Validators
{
    public class UserRegisterValidator : AbstractValidator<UserRegisterRequest>
    {
        public UserRegisterValidator()
        {
            RuleFor(x => x.Username).NotEmpty()
                .WithMessage("Insira o nome de usuario");
            RuleFor(x => x.Password).NotEmpty()
                .WithMessage("Insira uma senha");
            RuleFor(x => x.Email).EmailAddress()
                .WithMessage("Insira um email valido");
        }
    }
}
