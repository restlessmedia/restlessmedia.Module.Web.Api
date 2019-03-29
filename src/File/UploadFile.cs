using System;
using System.Net.Http;

namespace restlessmedia.Module.Web.Api.File
{
  public class UploadFile
  {
    public UploadFile(MultipartFileData fileData)
    {
      _fileData = fileData ?? throw new ArgumentNullException(nameof(fileData));
    }

    public string FileName
    {
      get
      {
        return _fileData.LocalFileName;
      }
    }

    private readonly MultipartFileData _fileData;
  }
}