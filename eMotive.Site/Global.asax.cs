﻿using System.Web.Mvc;
using System.Web.Routing;
using FluentValidation.Attributes;
using FluentValidation.Mvc;

namespace eMotive.MMI
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            DataAnnotationsModelValidatorProvider.AddImplicitRequiredAttributeForValueTypes = false;
            ModelValidatorProviders.Providers.Add(new FluentValidationModelValidatorProvider(new AttributedValidatorFactory()));

        }
    }
}