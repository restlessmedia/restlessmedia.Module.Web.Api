using System.Collections.Generic;
using System.Linq;

namespace restlessmedia.Module.Web.Api.File
{
  public class UploadFileResults
  {
    public UploadFileResults(UploadStreamProvider provider)
    {
      Files = provider.FileData.Select(x => new UploadFile(x));
    }

    public readonly IEnumerable<UploadFile> Files;
  }
}