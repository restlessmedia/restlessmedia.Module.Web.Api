using System.Net;
using System.Net.Http;
using System.Web.Http.Filters;

namespace restlessmedia.Module.Web.Api
{
  public class HttpCacheAttribute : ActionFilterAttribute
  {
    protected HttpCacheAttribute(bool isPublic = true)
    {
      Options = new HttpCacheOptions(isPublic);
    }

    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
      base.OnActionExecuted(actionExecutedContext);

      if(actionExecutedContext.Response != null)
      {
        Options.Apply(actionExecutedContext.Response);
      }
    }

    protected HttpResponseMessage NotModified()
    {
      return new HttpResponseMessage(HttpStatusCode.NotModified);
    }

    protected virtual HttpCacheOptions Options { get; private set; }
  }
}