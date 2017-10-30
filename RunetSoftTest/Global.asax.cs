using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RunetSoftTest
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ClientDataTypeModelValidatorProvider.ResourceClassKey = "MyLocalizatedMessages";
            DefaultModelBinder.ResourceClassKey = "MyLocalizatedMessages";
        }
    }
}
