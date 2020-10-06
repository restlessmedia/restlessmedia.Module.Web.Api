using System;
using System.Web.Http;

namespace restlessmedia.Module.Web.Api.Controllers
{
  [RoutePrefix("api/entity")]
  public class IEntityController : ApiController
  {
    public IEntityController(IEntityService entityService)
    {
      _entityService = entityService ?? throw new ArgumentNullException(nameof(entityService));
    }

    [HttpPost]
    [Route("{source}/{sourceId}/move/{target}/{targetId}/{direction}")]
    public IHttpActionResult Move(EntityType source, int sourceId, EntityType target, int targetId, MoveDirection direction)
    {
      try
      {
        _entityService.Move(source, target, sourceId, targetId, direction);
        return Ok();
      }
      catch (Exception e)
      {
        return InternalServerError(e);
      }
    }

    [HttpPost]
    [Route("{source}/{sourceId}/move/{targetId}/{direction}")]
    public IHttpActionResult Move(EntityType source, int sourceId, int targetId, MoveDirection direction)
    {
      return Move(source, sourceId, source, targetId, direction);
    }

    private readonly IEntityService _entityService;
  }
}