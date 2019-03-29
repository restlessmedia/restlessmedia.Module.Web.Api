using System.IO;
using System.Net.Http.Headers;

namespace restlessmedia.Module.Web.Api.Upload
{
  public interface IUploadHandler
  {
    Stream GetStream(HttpContentHeaders headers);

    void Done();
  }
}