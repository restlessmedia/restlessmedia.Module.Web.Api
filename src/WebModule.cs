﻿using Autofac;
using Autofac.Integration.WebApi;
using restlessmedia.Module.Web.Api.Attributes;
using restlessmedia.Module.Web.Api.MediaType;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Http;

namespace restlessmedia.Module.Web.Api
{
  public class WebModule : WebModuleBase
  {
    public override void OnStart(HttpConfiguration httpConfiguration, ContainerBuilder builder, IEnumerable<IWebModule> webModules)
    {
      httpConfiguration.Formatters[0] = new JsonNetFormatter(); // insert at 0 so it runs before any others
      httpConfiguration.MapHttpAttributeRoutes();
      httpConfiguration.Filters.Add(new ExceptionHandlingAttribute());

      // default routes
      httpConfiguration.Routes.MapHttpRoute(
            "DefaultApi1",
            "api/{controller}/{action}"
        );
      httpConfiguration.Routes.MapHttpRoute(
            "DefaultApi2",
            "api/{controller}"
        );

      // find assemblies from all loaded web modules
      // adding the current assembly
      Assembly[] webModuleAssemblies = webModules.Select(x => x.GetType().Assembly).Union(GetType().Assembly).ToArray();

      // register
      builder.RegisterApiControllers(webModuleAssemblies);
    }

    public override void OnStart(HttpConfiguration httpConfiguration, IContainer container, IEnumerable<IWebModule> webModules)
    {
      httpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
    }
  }
}