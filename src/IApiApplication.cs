using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Formatting;

namespace restlessmedia.Module.Web.Api
{
  public interface IApiApplication
  {
    void RegisterFormatters(MediaTypeFormatterCollection formatters);

    void RegisterHandlers(Collection<DelegatingHandler> handlers);
  }
}