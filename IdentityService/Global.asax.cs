using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using WebApiContrib.MessageHandlers;

namespace IdentityService
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);

            //MVC Filters
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);

            // HTTP Filters:
            // Add require API token Http filter if appSettings RequireApiToken is true:
            var requireApiToken = ConfigurationManager.AppSettings["RequireApiToken"] as string;
            if (!string.IsNullOrEmpty(requireApiToken) && requireApiToken.Equals("true"))
                FilterConfig.RegisterGlobalFilters(GlobalConfiguration.Configuration.Filters);

            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var json = GlobalConfiguration.Configuration.Formatters.JsonFormatter;
            json.SerializerSettings.TypeNameHandling = TypeNameHandling.Auto;
            //json.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore; //Enable this to only return fields that are not null.
            //json.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver(); //Enable this to convert output to camel case.

            // Uncomment this to add application/json to the media type mappings by default.
           // GlobalConfiguration.Configuration.Formatters.JsonFormatter.MediaTypeMappings.Add(new QueryStringMapping("a", "b", "application/json"));

            // Add require https check message handler if appSettings RequireHttps is true:
            var requireHttps = ConfigurationManager.AppSettings["RequireHttps"] as string;
            if (!string.IsNullOrEmpty(requireHttps) && requireHttps.Equals("true"))
                GlobalConfiguration.Configuration.MessageHandlers.Add(new RequireHttpsHandler());
        }
    }
}