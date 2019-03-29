using System.IO;
using System.Net.Http.Headers;

namespace restlessmedia.Module.Web.Api.File
{
  public interface IUploadStream
  {
    Stream GetStream(HttpContentHeaders headers, string fileName);
  }
}