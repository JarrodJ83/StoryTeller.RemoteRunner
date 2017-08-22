using System;
using System.Web.Http;
using Microsoft.Owin.Hosting;
using Owin;
using StoryTeller.RemoteRunner.Controllers;

namespace StoryTeller.RemoteRunner.Api
{
    public class RemoveRunService
    {
        private IDisposable _webApp;

        public void Start(string url)
        {
            _webApp = WebApp.Start<StartOwin>(url);
        }

        public void Stop()
        {
            _webApp.Dispose();
        }
    }

    public class StartOwin
    {
        public void Configuration(IAppBuilder appBuilder)
        {
            var config = new HttpConfiguration();
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );

            var test = new RunsController();


            config.MapHttpAttributeRoutes();

            appBuilder.UseWebApi(config);
        }
    }
}
