using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Controllers;

namespace restlessmedia.Module.Web.Api.Attributes
{
  public class EntityCacheAttribute : ETagCacheAttribute
  {
    public EntityCacheAttribute()
      : base()
    { }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      int key;

      if (TryGetKey(actionContext.ActionArguments, out key))
      {
        ETagDate = Resolve<IEntityService>(actionContext).UpdatedDate(Type, key);
      }

      base.OnActionExecuting(actionContext);
    }

    public EntityType Type { get; set; }

    public string KeyName { get; set; }

    private bool TryGetKey(IDictionary<string, object> routeValues, out int key)
    {
      key = 0;

      if (string.IsNullOrEmpty(KeyName) || !routeValues.ContainsKey(KeyName) || routeValues[KeyName] == null)
      {
        return false;
      }

      return int.TryParse(routeValues[KeyName].ToString(), out key);
    }

    private T Resolve<T>(HttpActionContext actionContext)
    {
      return (T)actionContext.Request.GetDependencyScope().GetService(typeof(T));
    }
  }
}