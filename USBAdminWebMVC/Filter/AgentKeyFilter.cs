using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using USBCommon;

namespace USBAdminWebMVC
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class AgentKeyFilter : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            //Debugger.Break();

            var headers = context.HttpContext.Request.Headers;
            var key = headers["AgentKey"][0];
            if (key != USBAdminHelp.AgentKey)
            {
                //context.Result = new JsonResult(new AgentHttpResponseResult(false, "Agent key no access right."));
                context.Result = new BadRequestObjectResult("no access right.");
                return;
            }

            await next();
        }
    }
}
