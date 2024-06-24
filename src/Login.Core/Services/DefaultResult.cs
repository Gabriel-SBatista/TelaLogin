using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Core.Services
{
    public class DefaultResult<T>
    {
        public bool Success { get; set; }
        public IEnumerable<string>? Messages { get; set; }
        public T? Data { get; set; }

        public DefaultResult(T data)
        {
            Data = data;
            Success = true;
            Messages = null;
        }

        public DefaultResult(string message)
        {
            Success = false;
            Messages = new List<string> { message };
        }

        public DefaultResult(bool success)
        {
            Success = success;
        }

        public DefaultResult(List<ValidationFailure> errors)
        {
            Messages = errors.Select(x => x.ErrorMessage);
            Success = false;
        }
    }
}
