using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Login.Core.Utils
{
    public class LinkGenerator
    {
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IHttpContextAccessor _httpContextAcessor;

        public LinkGenerator(IUrlHelperFactory urlHelperFactory, IHttpContextAccessor httpContextAcessor)
        {
            _urlHelperFactory = urlHelperFactory;
            _httpContextAcessor = httpContextAcessor;
        }

        public string GenerateEmailConfirmationLink(string token)
        {
            var actionContext = new ActionContext(_httpContextAcessor.HttpContext, new RouteData(), new ActionDescriptor());
            var urlHelper = _urlHelperFactory.GetUrlHelper(actionContext);

            return urlHelper.Action("ConfirmEmail", "Account", new { token }, _httpContextAcessor.HttpContext.Request.Scheme);
        }
    }
}
