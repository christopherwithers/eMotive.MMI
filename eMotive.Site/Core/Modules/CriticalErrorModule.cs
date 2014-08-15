using System;
using System.Web;
using System.Web.Mvc;
using eMotive.Models.Objects.StatusPages;
using eMotive.Services.Interfaces;
using Extensions;
using ServiceStack.WebHost.Endpoints;

namespace eMotive.MMI.Core.Modules
{
    public class CriticalErrorModule : IHttpModule
    {
        private INotificationService _logService;

        public void Init(HttpApplication context)
        {
            context.PreSendRequestContent += LogRequest;
        }

        private void LogRequest(object sender, EventArgs e)
        {
            var criticalErrors = _logService.FetchErrors();

          /*  if (criticalErrors.HasContent())
            {
                var helper = new UrlHelper(filterContext.RequestContext);



                var controller = filterContext.RouteData.DataTokens["controller"] ?? string.Empty;
                var action = filterContext.RouteData.DataTokens["action"] ?? string.Empty;
                var area = filterContext.RouteData.DataTokens["area"] ?? string.Empty;

                var url = helper.Action("Error", "Home", new { area = (!string.IsNullOrEmpty(area.ToString()) && area.ToString() == "Admin") ? "Admin" : "" });

                var errorView = new ErrorView
                {
                    Referrer = string.Format("/{0}/{1}", area, controller),
                    ControllerName = controller.ToString(),
                    Errors = criticalErrors
                };

                filterContext.Controller.TempData["CriticalErrors"] = errorView;

                filterContext.Result = new RedirectResult(url);
            }*/

           // http://geekswithblogs.net/mrsteve/archive/2011/04/06/httpmodule-to-redirect-a-user-request-when-uploaded-file-too-big.aspx
        }

        public void Dispose()
        {
            //do we need to do anything here?
        }
    }
}