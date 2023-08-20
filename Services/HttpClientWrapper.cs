using System.Net.Http.Headers;

namespace RedditTracker.Services
{
    public class HttpClientWrapper : IHttpClientWrapper
    {
        private readonly HttpClient _httpClient;

        public HttpClientWrapper(HttpClient httpClient)
        {
            _httpClient = httpClient;
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
        }

        public Task<HttpResponseMessage> GetAsync(string uri, AuthenticationHeaderValue authenticationHeader)
        {
            _httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;
            return _httpClient.GetAsync(uri);
        }

        public Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, AuthenticationHeaderValue authenticationHeader)
        {
            _httpClient.DefaultRequestHeaders.Authorization = authenticationHeader;
            return _httpClient.PostAsync(uri, content);
        }
    }
}
