using System.Web.Http.Results;
using System.Web.Http.ModelBinding;
using System.Net;

namespace System.Web.Http
{
  public static class ControllerExtensions
  {
    public static IHttpActionResult TryResult(this ApiController controller, Action fn)
    {
      try
      {
        fn();
      }
      catch (Exception e)
      {
        return Error(controller, e);
      }

      return new OkResult(controller);
    }

    public static IHttpActionResult Error(this ApiController controller, Exception exception, bool includeDetail = false)
    {
      if (includeDetail)
      {
        return new ExceptionResult(exception, true, controller.Configuration.Services.GetContentNegotiator(), controller.Request, controller.Configuration.Formatters);
      }

      return new ExceptionResult(exception, controller);
    }

    public static IHttpActionResult ModelStateResult(this ApiController controller)
    {
      return new NegotiatedContentResult<ModelStateDictionary>(HttpStatusCode.BadRequest, controller.ModelState, controller);
    }
  }
}