using restlessmedia.Module.Security;

namespace restlessmedia.Module.Web.Api.Attributes
{
  /// <summary>
  /// There is always a system admin role with admin actitivity and basic access.  This is the wrapper check for it.
  /// </summary>
  public class AdminAttribute : ActivityAttribute
	{
		public AdminAttribute()
      : base(SystemActivity.Admin, ActivityAccess.Basic) { }
	}
}