using restlessmedia.Module.Web.Api.MediaType;
using restlessmedia.Test;
using System.IO;
using Xunit;

namespace restlessmedia.Module.Web.Api.UnitTest.MediaType
{
  public class JsonNetFormatterTests
  {
    public JsonNetFormatterTests()
    {
      _jsonNetFormatter = new JsonNetFormatter();
    }

    [Fact]
    public void supports_read_and_write()
    {
      _jsonNetFormatter.CanReadType(typeof(Test)).MustBeTrue();
      _jsonNetFormatter.CanWriteType(typeof(Test)).MustBeTrue();
    }

    [Fact]
    public void writes_json_to_stream()
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        Test test = new Test("the-name");
        _jsonNetFormatter.WriteToStreamAsync(typeof(Test), test, memoryStream, null, null);
        memoryStream.Seek(0, SeekOrigin.Begin);
        StreamReader streamReader = new StreamReader(memoryStream);
        streamReader.ReadToEnd().MustBe("{\"name\":\"the-name\"}");
      }
    }

    [Fact]
    public async void reads_json_from_stream()
    {
      using (MemoryStream memoryStream = new MemoryStream())
      {
        StreamWriter streamWriter = new StreamWriter(memoryStream);
        streamWriter.Write("{\"name\":\"the-name\"}");
        streamWriter.Flush();
        memoryStream.Seek(0, SeekOrigin.Begin);
        object result = await _jsonNetFormatter.ReadFromStreamAsync(typeof(Test), memoryStream, null, null);
        result.MustBeA<Test>();
        ((Test)result).Name.MustBe("the-name");
      }
    }

    private struct Test
    {
      public Test(string name)
      {
        Name = name;
      }

      public readonly string Name;
    }

    private readonly JsonNetFormatter _jsonNetFormatter;
  }
}
