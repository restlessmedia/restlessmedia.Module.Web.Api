using System.Web.Http.Filters;

namespace restlessmedia.Module.Web.Api
{
  public class AddCustomHeaderFilter : ActionFilterAttribute
  {
    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      if (actionExecutedContext.Response != null)
      {
        System.Net.Http.ObjectContent content = actionExecutedContext.Response.Content as System.Net.Http.ObjectContent;

        if (content != null && content.ObjectType != null)
        {
          actionExecutedContext.Response.Headers.Add("X-Object-Type", content.ObjectType.Name);
        }
      }

      base.OnActionExecuted(actionExecutedContext);
    }
  }
}