using restlessmedia.Module.File;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace restlessmedia.Module.Web.Api.Upload
{
  public class FileUploadHandler : DefaultUploadHandler
  {
    public FileUploadHandler(IFileService fileService, IDiskStorageProvider storageProvider, EntityType entityType, int entityId, string path)
    {
      if (string.IsNullOrEmpty(path))
      {
        throw new ArgumentNullException(nameof(path));
      }

      _fileService = fileService ?? throw new ArgumentNullException(nameof(fileService));
      _storageProvider = storageProvider ?? throw new ArgumentNullException(nameof(storageProvider));
      _entityType = entityType;
      _entityId = entityId;
      _path = path;
      FileData = new Collection<MultipartFileData>();
    }

    public override void Done()
    {
      if (FileData.Count == 0)
      {
        return;
      }

      foreach (MultipartFileData file in FileData)
      {
        _fileService.Create(_entityType, _entityId, file.LocalFileName, file.Headers.ContentType.MediaType);
      }
    }

    public override Stream GetStream(HttpContentHeaders headers)
    {
      if (!IsValidUpload(headers))
      {
        throw new Exception("Not a valid upload, check file extension(s).");
      }

      string fileName = GetFileName(headers.ContentDisposition);
      FileData.Add(new MultipartFileData(headers, fileName));
      string contentType = headers.ContentType != null ? headers.ContentType.MediaType : null;
      return _storageProvider.Create(_path, fileName, contentType);
    }

    protected override bool IsValidType(MediaTypeHeaderValue contentType)
    {
      if (contentType == null)
      {
        return false;
      }

      return new[] { "image/jpeg", "image/jpg", "image/png", "image/gif" }.Contains(contentType.MediaType, StringComparer.OrdinalIgnoreCase);
    }

    protected override bool IsValidExtension(string extension)
    {
      return new[] { "gif", "jpg", "jpeg", "png" }.Contains(extension, StringComparer.OrdinalIgnoreCase);
    }

    protected override string GetFileName(ContentDispositionHeaderValue contentDisposition)
    {
      string fileName = base.GetFileName(contentDisposition);

      if (string.IsNullOrEmpty(fileName))
      {
        return null;
      }

      string extension = fileName.Split('.').Last();

      return string.Concat(Guid.NewGuid(), ".", extension);
    }

    private readonly Collection<MultipartFileData> FileData;

    private readonly IFileService _fileService;

    private readonly IDiskStorageProvider _storageProvider;

    private readonly EntityType _entityType;

    private readonly int _entityId;

    private readonly string _path;
  }
}