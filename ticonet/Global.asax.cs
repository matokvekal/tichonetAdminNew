using FluentScheduler;
using IdentitySample;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ticonet.Scheduler;
using log4net;

namespace ticonet
{
    public class MvcApplication : System.Web.HttpApplication
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // Scheduler
            TaskManager.TaskFactory = new NinjectTaskFactory();
            TaskManager.UnobservedTaskException += TaskException.TaskManager_UnobservedTaskException;
            TaskManager.Initialize(new TaskRegistry());
            #region log4net
            string file_path = "";
            try
            {
                file_path = System.IO.Path.Combine(Server.MapPath("~"), "log4net.config");

                log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo(file_path));
                if (!log.Logger.Repository.Configured)
                {
                    System.Diagnostics.Debug.WriteLine("Fail to configure log4net with config = '{0}'", file_path);
                }

            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(string.Format("log4net is not configured. Config file = '{0}'. {1}", file_path, ex.Message));
            }
            #endregion

        }
    }
}
