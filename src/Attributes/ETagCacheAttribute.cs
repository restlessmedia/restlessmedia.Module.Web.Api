using System;
using System.Web.Http.Controllers;

namespace restlessmedia.Module.Web.Api
{
  public class ETagCacheAttribute : HttpCacheAttribute
  {
    public ETagCacheAttribute()
    {
      Options.IsPublic = true;
    }

    public ETagCacheAttribute(string eTag)
        : this()
    {
      ETag = eTag;
    }

    public ETagCacheAttribute(DateTime date)
        : this()
    {
      ETagDate = date;
    }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if (Options.ETagMatches(actionContext.Request))
      {
        actionContext.Response = NotModified();
      }
      else
      {
        base.OnActionExecuting(actionContext);
      }
    }

    protected string ETag
    {
      get
      {
        return Options.ETag;
      }
      set
      {
        Options.ETag = value;
      }
    }

    protected DateTime? ETagDate
    {
      get
      {
        return Options.LastModified;
      }
      set
      {
        ETag = value.HasValue ? value.Value.Ticks.ToString() : null;
        Options.LastModified = value;
      }
    }
  }
}