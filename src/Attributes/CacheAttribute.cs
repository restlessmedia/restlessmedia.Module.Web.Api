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
    public CacheAttribute()
    {
      _dependencyResolver = (actionContext, type) => actionContext.Request.GetDependencyScope().GetService(type);
    }

    public CacheAttribute(int hours)
      : this()
    {
      _hours = hours;
    }

    public CacheAttribute(int hours, Func<HttpActionContext, Type, object> dependencyResolver)
    {
      _dependencyResolver = dependencyResolver;
      _hours = hours;
    }

    public CacheAttribute(Func<HttpActionContext, Type, object> dependencyResolver)
    {
      _dependencyResolver = dependencyResolver;
    }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if (TryGetCacheItem(actionContext, out CacheItem item))
      {
        actionContext.Response = actionContext.Request.CreateResponse();
        actionContext.Response.Content = new StringContent(item.Content);

        string contentType = item.ContentType ?? actionContext.Request.Headers?.Accept?.FirstOrDefault().ToString();

        if (MediaTypeHeaderValue.TryParse(contentType, out MediaTypeHeaderValue mediaType))
        {
          actionContext.Response.Content.Headers.ContentType = mediaType;
        }

        return;
      }

      base.OnActionExecuting(actionContext);
    }

    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      if (actionExecutedContext?.Response?.IsSuccessStatusCode == true)
      {
        ICacheProvider cacheProvider = GetService<ICacheProvider>(actionExecutedContext.ActionContext);
        string key = CreateKey(actionExecutedContext.ActionContext);

        if (!cacheProvider.Exists(key))
        {
          TimeSpan? expires = _hours.HasValue ? TimeSpan.FromHours(_hours.Value) : (TimeSpan?)null;
          cacheProvider.Add(key, CreateCacheItem(actionExecutedContext.Response), expires);
        }
      }

      base.OnActionExecuted(actionExecutedContext);
    }

    private bool TryGetCacheItem(HttpActionContext actionContext, out CacheItem cacheItem)
    {
      cacheItem = null;

      // caching only suitable get requests
      if (actionContext?.Request?.Method == HttpMethod.Get)
      {
        ICacheProvider cacheProvider = GetService<ICacheProvider>(actionContext);
        string key = CreateKey(actionContext);
        if (cacheProvider.Exists(key))
        {
          cacheItem = cacheProvider.Get<CacheItem>(key);
        }
      }

      return cacheItem != null;
    }

    private T GetService<T>(HttpActionContext actionContext)
      where T : class
    {
      Type type = typeof(T);

      if (!(_dependencyResolver(actionContext, type) is T service))
      {
        throw new InvalidOperationException($"There is no {type.Name} registered, is the container missing a registration for {type.Name}?");
      }

      return service;
    }

    /// <summary>
    /// Creates the key used to get and set in the cache.
    /// </summary>
    /// <param name="actionContext"></param>
    /// <returns></returns>
    private string CreateKey(HttpActionContext actionContext)
    {
      ILicenseSettings licenseSettings = GetService<ILicenseSettings>(actionContext);
      const string separator = ":";
      MediaTypeWithQualityHeaderValue accept = actionContext?.Request?.Headers?.Accept?.FirstOrDefault();
      return string.Concat(licenseSettings.ApplicationName, separator, actionContext?.Request?.RequestUri?.OriginalString, separator, accept != null ? accept.ToString() : string.Empty);
    }

    /// <summary>
    /// Creates an instance of <see cref="CacheItem"/>.
    /// </summary>
    /// <param name="response"></param>
    /// <returns></returns>
    private static CacheItem CreateCacheItem(HttpResponseMessage response)
    {
      string content = response.Content.ReadAsStringAsync().Result;
      string contentType = response.Content.Headers.ContentType?.ToString();
      return new CacheItem(content, contentType);
    }

    private readonly Func<HttpActionContext, Type, object> _dependencyResolver;

    private readonly int? _hours;
  }
}