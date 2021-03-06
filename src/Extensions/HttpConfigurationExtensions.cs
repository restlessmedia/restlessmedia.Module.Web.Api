﻿using restlessmedia.Module.Web.Api;
using System.Collections.Generic;
using System.Reflection;

namespace System.Web.Http
{
  public static class HttpConfigurationExtensions
  {
    public static void AddAssembly(this HttpConfiguration httpConfiguration, Assembly assembly)
    {
      Dispatcher.IAssembliesResolver existingAssembliesResolver = GetAssembliesResolver(httpConfiguration);
      ICollection<Assembly> existingAssemblies = existingAssembliesResolver.GetAssemblies();
      if (!existingAssemblies.Contains(assembly))
      {
        AssembliesResolver assembliesResolver = new AssembliesResolver(existingAssemblies);
        assembliesResolver.Add(assembly);
        httpConfiguration.Services.Replace(typeof(Dispatcher.IAssembliesResolver), assembliesResolver);
      }
    }

    public static Dispatcher.IAssembliesResolver GetAssembliesResolver(this HttpConfiguration httpConfiguration)
    {
      return httpConfiguration.Services.GetService(typeof(Dispatcher.IAssembliesResolver)) as Dispatcher.IAssembliesResolver;
    }
  }
}