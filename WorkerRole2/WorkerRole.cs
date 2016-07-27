using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Microsoft.WindowsAzure.ServiceRuntime;
using Owin;

namespace WorkerRole2
{
    public class WorkerRole : RoleEntryPoint
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

            var endpoint = RoleEnvironment.CurrentRoleInstance.InstanceEndpoints["Endpoint1"];            

            string baseUri = $"{endpoint.Protocol}://{endpoint.IPEndpoint}";

            Trace.TraceInformation($"Starting OWIN at {baseUri}",
                "Information");

            _app = WebApp.Start<Startup>(new StartOptions(baseUri));
            return base.OnStart();
        }

        public override void OnStop()
        {
            _app?.Dispose();
            base.OnStop();
        }
    }

    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                "Default",
                "{controller}/{id}",
                new {id = RouteParameter.Optional});

            app.UseWebApi(config);
        }
    }
}