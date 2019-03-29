using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace restlessmedia.Module.Web.Api.MediaType
{
  public class ApiJsonSettings : JsonSerializerSettings
  {
    public ApiJsonSettings()
    {
      ContractResolver = new CamelCasePropertyNamesContractResolver();
      Formatting = Formatting.None;
      ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
    }
  }
}