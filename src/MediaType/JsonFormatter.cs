using Newtonsoft.Json;
using restlessmedia.Module.File;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace restlessmedia.Module.Web.Api.MediaType
{
  public class JsonNetFormatter : MediaTypeFormatter
  {
    public JsonNetFormatter(JsonSerializerSettings settings, Encoding encoding)
    {
      _settings = settings ?? throw new ArgumentNullException(nameof(settings));
      _encoding = encoding;
      SupportedMediaTypes.Add(new MediaTypeHeaderValue(ContentType.Json));
    }

    public JsonNetFormatter()
      : this(new ApiJsonSettings(), Encoding.UTF8) { }

    public override bool CanReadType(Type type)
    {
      return true;
    }

    public override bool CanWriteType(Type type)
    {
      return true;
    }

    public override Task<object> ReadFromStreamAsync(Type type, Stream readStream, HttpContent content, IFormatterLogger formatterLogger)
    {
      using (StreamReader reader = new StreamReader(readStream))
      {
        return Task.FromResult(JsonConvert.DeserializeObject(reader.ReadToEnd(), type));
      }
    }

    public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
    {
      string serializedValue = JsonConvert.SerializeObject(value, _settings);
      byte[] data = (_encoding ?? Encoding.UTF8).GetBytes(serializedValue);
      writeStream.Write(data, 0, data.Length);
      return Task.FromResult(writeStream);
    }

    private readonly JsonSerializerSettings _settings;

    private readonly Encoding _encoding;
  }
}