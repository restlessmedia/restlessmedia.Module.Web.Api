using restlessmedia.Module.Configuration;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;

namespace restlessmedia.Module.Web.Api.Attributes
{
  [Serializable]
  public class CacheItem
  {
    public CacheItem() { }

    public CacheItem(string content, string contentType)
    {
      Content = content;
      ContentType = contentType;
    }

    public string Content;

    public string ContentType;

    public static string GetKey(HttpActionContext actionContext, ICacheProvider cacheProvider, ILicenseSettings licenseSettings)
    {
      const string separator = ":";
      MediaTypeWithQualityHeaderValue accept = actionContext.Request.Headers.Accept.FirstOrDefault();
      return string.Concat(licenseSettings.ApplicationName, separator, actionContext.Request.RequestUri.OriginalString, separator, accept != null ? accept.ToString() : string.Empty);
    }

    public static CacheItem FromResponse(HttpResponseMessage response)
    {
      string content = response.Content.ReadAsStringAsync().Result;
      string contentType = response.Content.Headers.ContentType != null ? response.Content.Headers.ContentType.ToString() : null;
      return new CacheItem(content, contentType);
    }
  }
}