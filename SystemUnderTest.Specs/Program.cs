using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using System.Web.Http.Dispatcher;
using Microsoft.Owin.Hosting;
using Owin;
using StoryTeller.RemoteRunner.Api;
using Topshelf;
using StoryTeller.RemoteRunner.Controllers;

namespace SystemUnderTest.Specs
{
    class Program
    {
        public static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<RemoveRunService>(s =>
                {
                    s.ConstructUsing(() => new RemoveRunService());
                    s.WhenStarted(service => service.Start("http://localhost:9000"));
                    s.WhenStopped(service => service.Stop());
                });
            });

            if (Debugger.IsAttached)
            {
                Console.ReadLine();
            }
        }

        //public class OwinService
        //{
        //    private IDisposable _webApp;

        //    public void Start(string url)
        //    {
        //        _webApp = WebApp.Start<StartOwin>(url);
        //    }

        //    public void Stop()
        //    {
        //        _webApp.Dispose();
        //    }
        //}

        //public class StartOwin
        //{
        //    public void Configuration(IAppBuilder appBuilder)
        //    {
        //        var config = new HttpConfiguration();
        //        config.Routes.MapHttpRoute(
        //            name: "DefaultApi",
        //            routeTemplate: "api/{controller}/{id}",
        //            defaults: new { id = RouteParameter.Optional }
        //        );

        //        var test = new RunsController();
  

        //        config.MapHttpAttributeRoutes();
                
        //        appBuilder.UseWebApi(config);
        //    }
        //}

        //public class MyNewAssembliesResolver : DefaultAssembliesResolver
        //{
        //    public override ICollection<Assembly> GetAssemblies()
        //    {
        //        ICollection<Assembly> baseAssemblies = base.GetAssemblies();
        //        List<Assembly> assemblies = new List<Assembly>(baseAssemblies);
        //        var controllersAssembly = Assembly.GetAssembly(typeof(RunsController));
        //        return baseAssemblies;
        //    }
        //}
    }

    //public class TestsController : ApiController
    //{
    //    public string Get(int id)
    //    {
    //        return "blah";
    //    }
    //}
}
