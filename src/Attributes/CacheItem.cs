using System;

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
  }
}