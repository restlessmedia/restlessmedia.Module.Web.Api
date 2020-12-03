using System.Net.Http.Headers;

namespace restlessmedia.Module.Web.Api.Extensions
{
  public static class HttpExtensions
  {
    public static bool IsAuthorized(this AuthenticationHeaderValue value, string username, string password, bool convertBase64 = true)
    {
      ParseAuthCredentials(value, out string authenticationUsername, out string authenticationPassword, convertBase64);

      return string.Compare(authenticationUsername, username) == 0 && string.Compare(authenticationPassword, password) == 0;
    }

    public static void ParseAuthCredentials(this AuthenticationHeaderValue value, out string username, out string password, bool convertBase64 = true)
    {
      if (value == null || string.IsNullOrEmpty(value.Parameter))
      {
        username = null;
        password = null;
      }
      else
      {
        Web.Extensions.HttpExtensions.ParseAuthCredentials(value.Parameter, out username, out password, convertBase64);
      }
    }
  }
}