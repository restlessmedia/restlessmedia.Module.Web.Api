using restlessmedia.Module.Extensions;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace restlessmedia.Module.Web.Api.File
{
  public class UploadStreamProvider : MultipartStreamProvider
  {
    public UploadStreamProvider(IUploadStream uploadStream)
    {
      _uploadStream = uploadStream ?? throw new ArgumentNullException(nameof(uploadStream));
      FormData = new NameValueCollection();
      FileData = new Collection<MultipartFileData>();
      _isFormData = new Collection<bool>();
    }

    public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
    {
      ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;
       
      if (contentDisposition == null)
      {
        throw new InvalidOperationException("No content disposition");
      }

      return GetStream(headers);
    }

    public override async Task ExecutePostProcessingAsync()
    {
      foreach (HttpContent httpContent in Contents.Where((HttpContent content, int index) => _isFormData[index]))
      {
        ContentDispositionHeaderValue contentDisposition = httpContent.Headers.ContentDisposition;
        string name = contentDisposition.Name.Unquote() ?? string.Empty;
        string value = await httpContent.ReadAsStringAsync();
        FormData.Add(name, value);
      };
    }

    public Collection<MultipartFileData> FileData { get; private set; }

    public NameValueCollection FormData { get; private set; }

    private Stream GetStream(HttpContentHeaders headers)
    {
      string fileName = GetFileName(headers.ContentDisposition);
      bool hasFileName = !string.IsNullOrEmpty(fileName);
      Stream stream = hasFileName ? _uploadStream.GetStream(headers, fileName) : new MemoryStream();

      _isFormData.Add(!hasFileName);

      if (hasFileName)
      {
        FileData.Add(new MultipartFileData(headers, fileName));
      }

      return stream;
    }

    public static string GetFileName(ContentDispositionHeaderValue contentDisposition)
    {
      string fileName = contentDisposition.FileName;

      if (string.IsNullOrEmpty(fileName))
      {
        return fileName;
      }

      return fileName.StripInvalidFileNameChars();
    }

    private readonly Collection<bool> _isFormData;

    private readonly IUploadStream _uploadStream;
  }
}