namespace restlessmedia.Module.Web.Api.Attributes
{
  public class WeekCacheAttribute : ClientCacheAttribute
  {
    public WeekCacheAttribute(int count = 1)
      : base(WeekInHours(count)) { }

    private static int WeekInHours(int count)
    {
      return (24 * 7) * count;
    }
  }
}