using System.Collections.Generic;
using System.Web.Http;
using System.Web.Http.Routing;

namespace restlessmedia.Module.Web.Api.Extensions
{
  public static class RouteExtensions
  {
    public static IHttpRoute MapHttpRoute(this HttpRouteCollection routes, string name, string routeTemplate, object defaults, object constraints, params string[] namespaces)
    {
      HttpRouteValueDictionary defaultsDictionary = new HttpRouteValueDictionary(defaults);
      HttpRouteValueDictionary constraintsDictionary = new HttpRouteValueDictionary(constraints);
      IDictionary<string, object> tokens = new Dictionary<string, object>()
      {
        { "Namespaces", namespaces }
      };
      IHttpRoute route = routes.CreateRoute(routeTemplate, defaultsDictionary, constraintsDictionary, tokens);
      routes.Add(name, route);
      return route;
    }

    public static IHttpRoute MapHttpRoute(this HttpRouteCollection routes, string name, string routeTemplate, object defaults, params string[] namespaces)
    {
      return routes.MapHttpRoute(name, routeTemplate, defaults, null, namespaces);
    }
  }
}