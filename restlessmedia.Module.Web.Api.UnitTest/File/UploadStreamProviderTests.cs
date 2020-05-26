using FakeItEasy;
using restlessmedia.Module.Web.Api.File;
using restlessmedia.Test;
using System.Net.Http;
using System.Net.Http.Headers;
using Xunit;

namespace restlessmedia.Module.Web.Api.UnitTest.File
{
  public class UploadStreamProviderTests : TestClass<UploadStreamProvider>
  {
    [Fact]
    public async void populates_formdata()
    {
      // set-up
      StringContent content = new StringContent("input-value");
      content.Headers.ContentDisposition = new ContentDispositionHeaderValue("asas")
      {
        Name = "input-field"
      };
      Instance.Contents.Add(content);

      // call

      // this is called by the framework and populates the form data from the content
      Instance.GetStream(content, content.Headers);
      await Instance.ExecutePostProcessingAsync();
      
      // assert
      Instance.FormData["input-field"].MustBe("input-value");
    }

    protected override UploadStreamProvider InstanceFactory()
    {
      IUploadStream uploadStream = A.Fake<IUploadStream>();
      return new UploadStreamProvider(uploadStream);
    }
  }
}
