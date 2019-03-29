using System;

namespace restlessmedia.Module.Web.Api.Attributes
{
  public class ClientCacheAttribute : ETagCacheAttribute
  {
    public ClientCacheAttribute(TimeSpan timeSpan)
      : base(DateTime.Now.Add(timeSpan))
    {
      Options.MaxAge = timeSpan;
    }

    public ClientCacheAttribute(int hours)
      : this(TimeSpan.FromHours(hours)) { }
  }
}