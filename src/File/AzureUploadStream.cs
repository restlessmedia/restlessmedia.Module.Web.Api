using System;
using System.IO;
using System.Net.Http.Headers;

namespace restlessmedia.Module.Web.Api.File
{
  public class AzureUploadStream : IUploadStream
  {
    public Stream GetStream(HttpContentHeaders headers, string fileName)
    {
      throw new NotImplementedException();
    }
  }
}