using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace USBAdminWebMVC
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AgentAuthenticateFilter : Attribute, IAsyncActionFilter
    {       
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Debugger.Break();

            Microsoft.AspNetCore.Http.IHeaderDictionary heads = context.HttpContext.Request.Headers;
            var key = heads["AgentCheckKey"][0];
            
            
            await next();
        }
    }
}
