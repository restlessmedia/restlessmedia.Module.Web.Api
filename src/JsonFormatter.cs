using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace restlessmedia.Module.Web.Api
{
  internal class JsonFormatter : MediaTypeFormatter
  {
    public JsonFormatter()
    {
      const string applicationJson = "application/json";
      SupportedMediaTypes.Add(new MediaTypeHeaderValue(applicationJson));
    }

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
      return Task.FromResult(Deserialize(type, readStream));
    }

    public override Task WriteToStreamAsync(Type type, object value, Stream writeStream, HttpContent content, TransportContext transportContext)
    {
      Serialize(value, writeStream);
      return Task.FromResult(writeStream);
    }

    private static object Deserialize(Type type, Stream stream)
    {
      using (StreamReader reader = new StreamReader(stream))
      {
        return JsonConvert.DeserializeObject(reader.ReadToEnd(), type);
      }
    }

    private static void Serialize<T>(T value, Stream stream, Encoding encoding = null)
    {
      byte[] data = (encoding ?? Encoding.UTF8).GetBytes(Serialize(value));
      stream.Write(data, 0, data.Length);
    }

    private static string Serialize<T>(T value)
    {
      return JsonConvert.SerializeObject(value, _defaultSerialiserOptions);
    }

    private static readonly JsonSerializerSettings _defaultSerialiserOptions = new JsonSerializerSettings
    {
      ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
      ContractResolver = new CamelCasePropertyNamesContractResolver(),
      Formatting = Formatting.None
    };
  }
}