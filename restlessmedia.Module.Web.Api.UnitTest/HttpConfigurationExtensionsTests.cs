using System;
using System.Linq;
using System.Reflection;
using System.Web.Http;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace restlessmedia.Module.Web.Api.UnitTest
{
  [TestClass]
  public class HttpConfigurationExtensionsTests
  {
    //[TestMethod]
    public void AddAssembly_adds_assembly_to_underlying_service()
    {
      HttpConfiguration httpConfiguration = new HttpConfiguration();
      Assembly assembly = Assembly.GetExecutingAssembly();

      httpConfiguration.AddAssembly(assembly);

      System.Web.Http.Dispatcher.IAssembliesResolver existingAssembliesResolver = httpConfiguration.Services.GetService(typeof(System.Web.Http.Dispatcher.IAssembliesResolver)) as System.Web.Http.Dispatcher.IAssembliesResolver;
      existingAssembliesResolver.GetAssemblies().Single(x => x == assembly);
    }
  }
}