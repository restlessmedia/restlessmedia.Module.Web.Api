using FakeItEasy;
using restlessmedia.Module.Configuration;
using restlessmedia.Module.Web.Api.Attributes;
using restlessmedia.Test;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;
using Xunit;

namespace restlessmedia.Module.Web.Api.UnitTest.Attributes
{
  public class CacheAttributeTests
  {
    public CacheAttributeTests()
    {
      _cacheProvider = A.Fake<ICacheProvider>();
      _licenseSettings = A.Fake<ILicenseSettings>();
      _dependencyResolver = A.Fake<Func<HttpActionContext, Type, object>>();
      _cacheAttribute = new CacheAttribute(_cachedHours, _dependencyResolver);
      _request = new HttpRequestMessage();

      HttpControllerContext httpControllerContext = new HttpControllerContext
      {
        Request = _request,
        Configuration = new HttpConfiguration()
      };

      _actionContext = new HttpActionContext
      {
        ControllerContext = httpControllerContext
      };

      HttpActionDescriptor httpActionDescriptor = A.Fake<HttpActionDescriptor>();

      _actionExecutedContext = new HttpActionExecutedContext
      {
        ActionContext = _actionContext
      };

      A.CallTo(() => _dependencyResolver(_actionContext, typeof(ICacheProvider))).Returns(_cacheProvider);
      A.CallTo(() => _dependencyResolver(_actionContext, typeof(ILicenseSettings))).Returns(_licenseSettings);
      A.CallTo(() => _licenseSettings.ApplicationName).Returns("testapplication");
    }

    [Fact]
    public async void cache_contents_when_found_are_set_on_the_response()
    {
      // set-up
      const string expectedResponse = "fooBar";
      const string contentType = "application/json";
      const string expectedKey = "testapplication:http://test.com/api/entity/get:application/json";
      
      _request.RequestUri = new Uri("http://test.com/api/entity/get");
      _request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));

      A.CallTo(() => _cacheProvider.Exists(expectedKey))
        .Returns(true);

      A.CallTo(() => _cacheProvider.Get<CacheItem>(expectedKey))
        .Returns(new CacheItem
        {
          Content = expectedResponse
        });

      // call
      _cacheAttribute.OnActionExecuting(_actionContext);

      // assert
      string actualResponse = await _actionContext.Response.Content.ReadAsStringAsync();
      _actionContext.Response.Content.Headers.ContentType.MediaType.MustBe(contentType);
      actualResponse.MustBe(expectedResponse);
    }

    [Fact]
    public async void response_not_overwritten_when_not_found_in_cache()
    {
      // set-up
      const string expectedResponse = "do-not-overwrite";
      const string expectedKey = "testapplication:http://test.com/api/entity/get:application/json";
      
      _request.RequestUri = new Uri("http://test.com/api/entity/get");
      _actionContext.Response = new HttpResponseMessage
      {
        Content = new StringContent(expectedResponse),
        StatusCode = HttpStatusCode.OK,
      };

      A.CallTo(() => _cacheProvider.Exists(expectedKey))
        .Returns(false);

      A.CallTo(() => _cacheProvider.Get<CacheItem>(expectedKey))
        .Returns(new CacheItem
        {
          Content = "as we return cache exists false, this should not be used"
        });

      // call
      _cacheAttribute.OnActionExecuting(_actionContext);

      // assert - the response should not be overwritten
      string actualResponse = await _actionContext.Response.Content.ReadAsStringAsync();
      actualResponse.MustBe(expectedResponse);
    }

    [Fact]
    public async void null_cache_entry_does_not_fail()
    {
      // set-up
      const string expectedResponse = "do-not-overwrite";
      const string expectedKey = "testapplication:http://test.com/api/entity/get:application/json";
      
      _request.RequestUri = new Uri("http://test.com/api/entity/get");
      
      A.CallTo(() => _cacheProvider.Exists(expectedKey))
        .Returns(true);

      A.CallTo(() => _cacheProvider.Get<CacheItem>(expectedKey))
        .Returns(null);

      _actionContext.Response = new HttpResponseMessage
      {
        Content = new StringContent(expectedResponse),
        StatusCode = HttpStatusCode.OK,
      };

      // call
      _cacheAttribute.OnActionExecuting(_actionContext);

      // assert - the response should not be overwritten as the cache returned null
      string actualResponse = await _actionContext.Response.Content.ReadAsStringAsync();
      actualResponse.MustBe(expectedResponse);
    }

    /// <summary>
    /// This checks we don't re-cache something that already has been pulled from the cache
    /// </summary>
    [Fact]
    public void cached_content_is_not_recached()
    {
      // set-up
      const string contentType = "application/json";
      const string expectedResponse = "do-not-overwrite";
      const string expectedKey = "testapplication:http://test.com/api/entity/get:application/json";
      
      _request.RequestUri = new Uri("http://test.com/api/entity/get");
      _request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
      _actionExecutedContext.Response = new HttpResponseMessage
      {
        Content = new StringContent(expectedResponse),
        StatusCode = HttpStatusCode.OK,
      };

      A.CallTo(() => _cacheProvider.Exists(expectedKey))
        .Returns(true);

      // call
      _cacheAttribute.OnActionExecuted(_actionExecutedContext);

      // assert - the cache should not be overwritten as the item was already in there
      A.CallTo(() => _cacheProvider.TryAdd(expectedKey, A<CacheItem>.Ignored, A<TimeSpan?>.Ignored))
        .MustNotHaveHappened();
    }

    [Fact]
    public void cached_content_is_cached_when_it_doesnt_exist()
    {
      // set-up
      const string contentType = "application/json";
      const string expectedResponse = "do-not-overwrite";
      const string expectedKey = "testapplication:http://test.com/api/entity/get:application/json";

      _request.RequestUri = new Uri("http://test.com/api/entity/get");
      _request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
      _actionExecutedContext.Response = new HttpResponseMessage
      {
        Content = new StringContent(expectedResponse),
        StatusCode = HttpStatusCode.OK,
      };

      A.CallTo(() => _cacheProvider.Exists(expectedKey))
        .Returns(false);

      // call
      _cacheAttribute.OnActionExecuted(_actionExecutedContext);

      // assert - the cache should not be overwritten as the item was already in there
      A.CallTo(() => _cacheProvider.TryAdd(expectedKey, A<CacheItem>.Ignored, A<TimeSpan?>.Ignored))
        .MustHaveHappened();
    }

    [Fact]
    public void cached_content_is_cached_usng_expiry()
    {
      // set-up
      const string contentType = "application/json";
      const string expectedResponse = "do-not-overwrite";
      const string expectedKey = "testapplication:http://test.com/api/entity/get:application/json";

      _request.RequestUri = new Uri("http://test.com/api/entity/get");
      _request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
      _actionExecutedContext.Response = new HttpResponseMessage
      {
        Content = new StringContent(expectedResponse),
        StatusCode = HttpStatusCode.OK,
      };

      A.CallTo(() => _cacheProvider.Exists(expectedKey))
        .Returns(false);

      // call
      _cacheAttribute.OnActionExecuted(_actionExecutedContext);

      // assert
      A.CallTo(() => _cacheProvider.TryAdd(expectedKey, A<CacheItem>.Ignored, TimeSpan.FromHours(_cachedHours)))
        .MustHaveHappened();
    }

    [Fact]
    public void doesnt_cache_non_200_responses()
    {
      // set-up
      const string contentType = "application/json";
      const string response = "an error";
      const string expectedKey = "testapplication:http://test.com/api/entity/get:application/json";

      _request.RequestUri = new Uri("http://test.com/api/entity/get");
      _request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(contentType));
      _actionExecutedContext.Response = new HttpResponseMessage
      {
        Content = new StringContent(response),
        StatusCode = HttpStatusCode.InternalServerError,
      };

      A.CallTo(() => _cacheProvider.Exists(expectedKey))
        .Returns(false);

      // call
      _cacheAttribute.OnActionExecuted(_actionExecutedContext);

      // assert
      A.CallTo(() => _cacheProvider.TryAdd(expectedKey, A<CacheItem>.Ignored, A<TimeSpan?>.Ignored))
        .MustNotHaveHappened();
    }

    private readonly HttpActionExecutedContext _actionExecutedContext;

    private readonly ICacheProvider _cacheProvider;

    private readonly ILicenseSettings _licenseSettings;

    private readonly CacheAttribute _cacheAttribute;

    private readonly Func<HttpActionContext, Type, object> _dependencyResolver;

    private readonly HttpActionContext _actionContext;

    private readonly HttpRequestMessage _request;

    const int _cachedHours = 10;
  }
}
