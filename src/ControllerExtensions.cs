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
        return new ExceptionResult(e, controller);
      }

      return new OkResult(controller);
    }

    public static IHttpActionResult ModelStateResult(this ApiController controller)
    {
      return new NegotiatedContentResult<ModelStateDictionary>(HttpStatusCode.BadRequest, controller.ModelState, controller);
    }
  }
}