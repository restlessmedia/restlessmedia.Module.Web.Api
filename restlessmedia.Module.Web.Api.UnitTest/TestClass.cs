using System;

namespace restlessmedia.Module.Web.Api.UnitTest
{
  public class TestClass<T>
  {
    public TestClass()
    {
      Instance = InstanceFactory();
    }

    public readonly T Instance;

    protected virtual T InstanceFactory()
    {
      return (T)typeof(T).GetConstructor(Type.EmptyTypes).Invoke(null);
    }
  }
}