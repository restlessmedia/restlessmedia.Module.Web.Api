using System;

namespace restlessmedia.Module.Web.Api.Attributes
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class DataModelAttribute : Attribute
  {
    public DataModelAttribute(Type type)
    {
      Type = type ?? throw new ArgumentNullException(nameof(type));
    }

    public string GetName()
    {
      return Name ?? Type.Name;
    }

    public string Name;

    public readonly Type Type;
  }
}