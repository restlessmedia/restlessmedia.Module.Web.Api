namespace restlessmedia.Module.Web.Api
{
  public interface IApiResult
  {
    bool Success { get; }

    object Data { get; }

    string Message { get; }
  }
}