using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Results;

namespace restlessmedia.Module.Web.Api.Extensions
{
  public static class RequestMessageExtensions
  {
    public static Task<IHttpActionResult> UploadAsync(this HttpRequestMessage request, DirectoryInfo uploadDirectory, Action<NameValueCollection, Collection<MultipartFileData>> done)
    {
      MultipartFormDataStreamProvider provider = new MultipartFormDataStreamProvider(uploadDirectory.FullName);
      return UploadAsync(request, provider, x => done(x.FormData, x.FileData));
    }

    public static Task<IHttpActionResult> UploadAsync(this HttpRequestMessage request, Func<HttpContentHeaders, Stream> streamHandler, Action done)
    {
      return UploadAsync(request, new StreamProvider(streamHandler), x => done());
    }

    public static async Task<IHttpActionResult> UploadAsync<T>(this HttpRequestMessage request, T provider, Action<T> done = null)
        where T : MultipartFileStreamProvider
    {
      if (!request.Content.IsMimeMultipartContent())
      {
        return BadRequest(request);
      }

      try
      {
        await request.Content.ReadAsMultipartAsync(provider);
      }
      catch (Exception e)
      {
        return BadRequest(request, e.GetBaseException().Message);
      }

      done?.Invoke(provider);

      return new OkResult(request);
    }

    public static string UnquotedFileName(this ContentDispositionHeaderValue contentDisposition)
    {
      string fileName = contentDisposition?.FileName;
      return string.IsNullOrEmpty(fileName) ? fileName : fileName.Trim('"');
    }

    private static IHttpActionResult BadRequest(HttpRequestMessage request, string message = null)
    {
      HttpResponseMessage response = request.CreateResponse(HttpStatusCode.BadRequest);

      if (!string.IsNullOrWhiteSpace(message))
      {
        response.Content = new StringContent(message);
      }

      return new ResponseMessageResult(response);
    }

    private class StreamProvider : MultipartFileStreamProvider
    {
      public StreamProvider(Func<HttpContentHeaders, Stream> streamHandler)
        : base("/")
      {
        _streamHandler = streamHandler;
      }

      public override Stream GetStream(HttpContent parent, HttpContentHeaders headers)
      {
        ContentDispositionHeaderValue contentDisposition = headers.ContentDisposition;

        if (contentDisposition != null && !string.IsNullOrEmpty(contentDisposition.FileName))
        {
          return _streamHandler(headers);
        }

        // not a valid file or formdata - satisfy the caller
        return new MemoryStream();
      }

      private readonly Func<HttpContentHeaders, Stream> _streamHandler;
    }
  }
}