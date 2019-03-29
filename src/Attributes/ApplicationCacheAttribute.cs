using System;

namespace restlessmedia.Module.Web.Api.Attributes
{
  public class ApplicationCacheAttribute : ETagCacheAttribute
  {
    public ApplicationCacheAttribute()
      : base(ApplicationStarted)
    { }

    private static DateTime ApplicationStarted = DateTime.Now;
  }
}