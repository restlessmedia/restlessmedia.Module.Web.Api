using restlessmedia.Module.Security;
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace restlessmedia.Module.Web.Api.Attributes
{
  public class ActivityAttribute : AuthorizeAttribute
	{
		public ActivityAttribute(string activity, ActivityAccess access = ActivityAccess.Basic)
		{
      if (string.IsNullOrWhiteSpace(activity))
      {
        throw new ArgumentNullException(nameof(activity));
      }

      _activity = activity;
      _access = access;
		}

    protected override bool IsAuthorized(HttpActionContext actionContext)
    {
      IUserInfo user = actionContext.RequestContext.Principal as IUserInfo;
      return user != null && Resolve<ISecurityService>(actionContext).Authorize(user, _activity, _access);
    }

    private T Resolve<T>(HttpActionContext actionContext)
    {
      return (T)actionContext.Request.GetDependencyScope().GetService(typeof(T));
    }

    private readonly string _activity;

    private readonly ActivityAccess _access;
	}
}