using restlessmedia.Module.Web.Api.Extensions;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;

namespace restlessmedia.Module.Web.Api.Upload
{
  public abstract class DefaultUploadHandler : IUploadHandler
  {
    public abstract Stream GetStream(HttpContentHeaders headers);

    public virtual void Done() { }

    protected abstract bool IsValidType(MediaTypeHeaderValue contentType);

    protected abstract bool IsValidExtension(string extension);

    protected virtual bool IsValidUpload(HttpContentHeaders headers)
    {
      if (headers == null)
      {
        return false;
      }

      return IsValidExtension(GetExtension(headers.ContentDisposition)) && IsValidType(headers.ContentType);
    }

    protected virtual string GetFileName(ContentDispositionHeaderValue contentDisposition)
    {
      return contentDisposition.UnquotedFileName();
    }

    private string GetExtension(ContentDispositionHeaderValue contentDisposition)
    {
      string fileName = contentDisposition.UnquotedFileName();

      if (string.IsNullOrEmpty(fileName))
      {
        return fileName;
      }

      return fileName.Split('.').Last();
    }
  }
}