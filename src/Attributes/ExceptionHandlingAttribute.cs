using Microsoft.ApplicationInsights;
using System.Web.Http.Filters;

namespace restlessmedia.Module.Web.Api.Attributes
{
  public class ExceptionHandlingAttribute : ExceptionFilterAttribute
  {
    public override void OnException(HttpActionExecutedContext context)
    {
      if (context != null && context.Exception != null)
      {
        TelemetryClient client = new TelemetryClient();
        client.TrackException(context.Exception);
      }

      base.OnException(context);
    }
  }
}