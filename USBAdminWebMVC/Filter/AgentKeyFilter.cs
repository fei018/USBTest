using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using USBCommon;

namespace USBAdminWebMVC
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AgentHttpKeyFilter : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Debugger.Break();

            var headers = context.HttpContext.Request.Headers;
            var key = headers["AgentHttpKey"][0];
            if (key != USBAdminHelp.AgentHttpKey)
            {
                //context.Result = new JsonResult(new AgentHttpResponseResult(false, "Agent key no access right."));
                context.Result = new BadRequestObjectResult("no access right.") { StatusCode = 401 };
                return;
            }

            await next();
        }
    }
}
