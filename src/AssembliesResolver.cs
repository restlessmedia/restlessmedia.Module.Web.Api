using System.Collections.Generic;
using System.Reflection;

namespace restlessmedia.Module.Web.Api
{
  public class AssembliesResolver : IAssembliesResolver
  {
    public AssembliesResolver()
    {
      _assemblies = _assemblies = new List<Assembly>(0);
    }

    public AssembliesResolver(ICollection<Assembly> assemblies)
    {
      _assemblies = assemblies;
    }

    public ICollection<Assembly> GetAssemblies()
    {
      return _assemblies;
    }

    public void Add(Assembly assembly)
    {
      _assemblies.Add(assembly);
    }

    private readonly ICollection<Assembly> _assemblies;
  }
}