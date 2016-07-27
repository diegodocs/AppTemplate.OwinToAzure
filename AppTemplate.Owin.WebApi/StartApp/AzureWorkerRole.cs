using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Microsoft.WindowsAzure.ServiceRuntime;
using Owin;

namespace AppTemplate.Owin.WebApi.StartApp
{
    public class AzureWorkerRole : RoleEntryPoint
    {
        private IDisposable _app;

        public override void Run()
        {
            Trace.TraceInformation("WebApiRole entry point called");

            while (true)
            {
                Thread.Sleep(10000);
                Trace.TraceInformation("Working");
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["EndpointDev"];
            string baseUri = $"{endpoint.Protocol}://{endpoint.IPEndpoint}";

            Trace.TraceInformation($"Starting OWIN at {baseUri}");            

            _app = WebApp.Start<Startup2Simple>(new StartOptions(baseUri));
            return base.OnStart();
        }

        public override void OnStop()
        {
            _app?.Dispose();
            base.OnStop();
        }
    }

    public class Startup2Simple
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "Default",
                "api/{controller}/{id}",
                new { id = RouteParameter.Optional });

            app.UseWebApi(config);
        }
    }
}