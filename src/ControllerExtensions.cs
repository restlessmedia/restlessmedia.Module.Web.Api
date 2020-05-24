using System.Web.Http.Results;

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
  }
}
