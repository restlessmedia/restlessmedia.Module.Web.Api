using Autofac;
using FakeItEasy;
using restlessmedia.Module.Web.Api.Controllers;
using restlessmedia.Test;
using System.Collections.Generic;
using System.Web.Http;
using Xunit;

namespace restlessmedia.Module.Web.Api.UnitTest
{
  public class WebModuleTests
  {
    public WebModuleTests()
    {
      _httpConfiguration = new HttpConfiguration();
      _containerBuilder = new ContainerBuilder();
      _webModule = new WebModule();
    }

    /// <summary>
    /// This to make sure we are 
    /// </summary>
    [Fact]
    public void controllers_are_registered()
    {
      // set-up
      IWebModule webModule = A.Fake<IWebModule>();
      IEnumerable<IWebModule> webModules = new[] { webModule };

      // call
      _webModule.OnStart(_httpConfiguration, _containerBuilder, webModules);

      // assert
      _containerBuilder.Build().IsRegistered<IEntityController>().MustBeTrue();
    }

    private readonly WebModule _webModule;

    private readonly HttpConfiguration _httpConfiguration;

    private readonly ContainerBuilder _containerBuilder;
  }
}
