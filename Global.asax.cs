using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Configuration;
using GenuinaBI.Service;

namespace GenuinaBI
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            log4net.Config.XmlConfigurator.Configure(new FileInfo(Server.MapPath("~/Web.config")));

            /* Identify database connection string through Web.config and GCS */
            if (ConfigurationManager.AppSettings["DBConfigSource"] == "GCS")
            {
                String gcsHost = ConfigurationManager.AppSettings["GCSHost"];
                String gcsPort = ConfigurationManager.AppSettings["GCSPort"];
                String dbConnectionStringTemplate = ConfigurationManager.AppSettings["DBConnectionStringTemplate"];

                Application["dbConnectionString"] = SecurityProviderService.getConnectionString(gcsHost, gcsPort, dbConnectionStringTemplate);
            }
            else
            {
                Application["dbConnectionString"] = ConfigurationManager.ConnectionStrings["GenuinaDBEntities"].ConnectionString;
            }
        }

        private static readonly log4net.ILog logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected void Application_Error(object sender, EventArgs e)
        {
            //Log exception
            Exception exception = Server.GetLastError();
            logger.Error(exception);

            //Redirect user
            HttpException httpException = exception as HttpException;

            if (httpException != null)
            {
                string action;

                switch (httpException.GetHttpCode())
                {
                    case 404:
                        // page not found
                        action = "HttpError404";
                        break;
                    case 500:
                        // server error
                        action = "HttpError500";
                        break;
                    default:
                        action = "General";
                        break;
                }

                // clear error on server
                //Clear error from response stream
                Response.Clear();
                Server.ClearError();

                Response.Redirect(String.Format("~/Error/{0}/?message={1}", action, exception.Message));
            }
        }
    }
}
