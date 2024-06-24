using FluentValidation;
using Login.Core.Requests;
using Login.Core.Validators;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Cross.DependencyInjection
{
    public static class ValidatorsDependencyInjection
    {
        public static IServiceCollection AddValidators(this IServiceCollection services)
        {
            services.AddScoped<IValidator<UserLoginRequest>, UserLoginValidator>();
            services.AddScoped<IValidator<UserRegisterRequest>, UserRegisterValidator>();

            return services;
        }
    }
}
