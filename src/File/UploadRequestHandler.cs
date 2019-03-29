using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace restlessmedia.Module.Web.Api.File
{
  public class UploadRequestHandler
  {
    public UploadRequestHandler(HttpRequestMessage request)
    {
      _request = request ?? throw new ArgumentNullException(nameof(request));
    }

    public Task<UploadFileResults> GetResults(IUploadStream uploadStream)
    {
      return Upload(uploadStream).ContinueWith(x => new UploadFileResults(x.Result));
    }

    public Task<UploadStreamProvider> Upload(IUploadStream uploadStream)
    {
      if (!_request.Content.IsMimeMultipartContent("form-data"))
      {
        throw new InvalidOperationException("UnsupportedMediaType");
      }

      UploadStreamProvider provider = new UploadStreamProvider(uploadStream);

      return _request.Content.ReadAsMultipartAsync(provider);
    }

    private readonly HttpRequestMessage _request;
  }
}