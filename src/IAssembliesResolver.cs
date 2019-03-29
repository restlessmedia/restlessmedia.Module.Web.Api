using System.Reflection;

namespace restlessmedia.Module.Web.Api
{
  public interface IAssembliesResolver : System.Web.Http.Dispatcher.IAssembliesResolver
  {
    void Add(Assembly assembly);
  }
}
