﻿using Autofac;
using Autofac.Integration.WebApi;
using restlessmedia.Module.Web.Api.Attributes;
using restlessmedia.Module.Web.Api.MediaType;
using System.Collections.Generic;
using System.Web.Http;

namespace restlessmedia.Module.Web.Api
{
  public class WebModule : IWebModule
  {
    public void OnStart(HttpConfiguration httpConfiguration, ContainerBuilder builder, IEnumerable<IWebModule> webModules)
    {
      httpConfiguration.Formatters[0] = new JsonNetFormatter(); // insert at 0 so it runs before any others
      httpConfiguration.MapHttpAttributeRoutes();
      httpConfiguration.Filters.Add(new ExceptionHandlingAttribute());
    }

    public void OnStart(HttpConfiguration httpConfiguration, IContainer container, IEnumerable<IWebModule> webModules)
    {
      httpConfiguration.DependencyResolver = new AutofacWebApiDependencyResolver(container);
    }
  }
}