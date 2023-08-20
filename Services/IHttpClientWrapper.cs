using System.Net.Http.Headers;

namespace RedditTracker.Services
{
    public interface IHttpClientWrapper
    {
        Task<HttpResponseMessage> GetAsync(string uri, AuthenticationHeaderValue authenticationHeader);
        Task<HttpResponseMessage> PostAsync(string uri, HttpContent content, AuthenticationHeaderValue authenticationHeader);
    }
}