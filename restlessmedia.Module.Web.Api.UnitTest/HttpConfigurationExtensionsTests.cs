using restlessmedia.Test;
using System.Reflection;
using System.Web.Http;
using Xunit;

namespace restlessmedia.Module.Web.Api.UnitTest
{
  public class HttpConfigurationExtensionsTests
  {
    [Fact]
    public void AddAssembly_adds_assembly_to_underlying_service()
    {
      HttpConfiguration httpConfiguration = new HttpConfiguration();
      Assembly assembly = Assembly.GetExecutingAssembly();

      httpConfiguration.AddAssembly(assembly);

      System.Web.Http.Dispatcher.IAssembliesResolver existingAssembliesResolver = httpConfiguration.Services.GetService(typeof(System.Web.Http.Dispatcher.IAssembliesResolver)) as System.Web.Http.Dispatcher.IAssembliesResolver;
      existingAssembliesResolver.GetAssemblies().Contains(assembly).MustBeTrue();
    }
  }
}