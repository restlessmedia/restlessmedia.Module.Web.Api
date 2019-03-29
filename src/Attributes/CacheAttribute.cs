using restlessmedia.Module.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace restlessmedia.Module.Web.Api.Attributes
{
  public class CacheAttribute : ActionFilterAttribute
  {
    public CacheAttribute() { }

    public CacheAttribute(int hours)
    {
      _hours = hours;
    }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      bool isCacheable = actionContext != null && actionContext.Request.Method == HttpMethod.Get;

      if (isCacheable)
      {
        ICacheProvider cacheProvider = GetService<ICacheProvider>(actionContext);
        ILicenseSettings licenseSettings = GetService<ILicenseSettings>(actionContext);
        string key = CacheItem.GetKey(actionContext, cacheProvider, licenseSettings);

        if (cacheProvider.Exists(key))
        {
          CacheItem item = cacheProvider.Get<CacheItem>(key);
          string contentType = item.ContentType ?? actionContext.Request.Headers.Accept.FirstOrDefault().ToString();
          actionContext.Response = actionContext.Request.CreateResponse();
          actionContext.Response.Content = new StringContent(item.Content);

          MediaTypeHeaderValue mediaType = null;

          if (MediaTypeHeaderValue.TryParse(contentType, out mediaType))
          {
            actionContext.Response.Content.Headers.ContentType = mediaType;
          }

          return;
        }
      }

      base.OnActionExecuting(actionContext);
    }

    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      bool canCache = actionExecutedContext != null && actionExecutedContext.Response != null && actionExecutedContext.Response.Content != null;

      if (canCache)
      {
        ICacheProvider cacheProvider = GetService<ICacheProvider>(actionExecutedContext.ActionContext);
        ILicenseSettings licenseSettings = GetService<ILicenseSettings>(actionExecutedContext.ActionContext);
        TimeSpan? duration = _hours.HasValue ? TimeSpan.FromHours(_hours.Value) : (TimeSpan?)null;
        cacheProvider.Add(CacheItem.GetKey(actionExecutedContext.ActionContext, cacheProvider, licenseSettings), CacheItem.FromResponse(actionExecutedContext.Response));
      }

      base.OnActionExecuted(actionExecutedContext);
    }


    private static T GetService<T>(HttpActionContext actionContext)
      where T : class
    {
      T service = actionContext.Request.GetDependencyScope().GetService(typeof(T)) as T;

      if (service == null)
      {
        throw new InvalidOperationException($"There is no CacheProvider registered for CacheAttribute.  Is the container missing a registration for ICacheProvider?");
      }

      return service;
    }

    private readonly int? _hours;
  }
}