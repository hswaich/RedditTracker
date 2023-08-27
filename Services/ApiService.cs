using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedditTracker.Structures;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;

namespace RedditTracker.Services
{
    public class ApiService : IApiService
    {
        private readonly IUtilityService _utilityService;
        private readonly IHttpClientWrapper _httpClientWrapper;

        private readonly string baseUrl;
        private readonly string accessTokenUrl;
        private readonly ApiKeys apiKeys;
        private readonly RedditAccount redditAccount;
        private AccessTokenResponse accessTokenResponse;
        
        public ResponseHeader ResponseHeader { get; set; }

        public ApiService(IConfiguration configuration, IUtilityService utilityService, IHttpClientWrapper httpClientWrapper)
        {
            _utilityService = utilityService;
            _httpClientWrapper = httpClientWrapper;

            apiKeys = new ApiKeys();
            redditAccount = new RedditAccount();
            accessTokenResponse = new AccessTokenResponse();
            baseUrl = configuration.GetValue<string>("BaseUrl");
            accessTokenUrl = configuration.GetValue<string>("AccessTokenUrl");
            configuration.GetSection("ApiKeys").Bind(apiKeys);
            configuration.GetSection("RedditAccount").Bind(redditAccount);
        }


        public async Task<string> GetToken()
        {
            // empty or expired
            if (string.IsNullOrEmpty(accessTokenResponse.access_token) || DateTime.Now >= accessTokenResponse.ExpiresAt)
            {
                return await GetNewToken();
            }
            else
            {
                return accessTokenResponse.access_token;
            }
        }

        private async Task<string> GetNewToken()
        {
            var authData = new FormUrlEncodedContent(new[]
            {
                        new KeyValuePair<string, string>("grant_type", "password"),
                        new KeyValuePair<string, string>("username", redditAccount.username),
                        new KeyValuePair<string, string>("password", redditAccount.password)
                    });

            var credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{apiKeys.clientId}:{apiKeys.clientSecret}"));
       
            HttpResponseMessage authResponse = await _httpClientWrapper.PostAsync(accessTokenUrl, authData, new AuthenticationHeaderValue("Basic", credentials));

            if (authResponse.IsSuccessStatusCode)
            {
                var authJson = await authResponse.Content.ReadAsStringAsync();
                accessTokenResponse = JsonConvert.DeserializeObject<AccessTokenResponse>(authJson);
                accessTokenResponse.ExpiresAt = DateTime.Now.AddSeconds(accessTokenResponse.expires_in);
                return accessTokenResponse.access_token;
            }
            else
            {
                return string.Empty;
            }
        }

        public async Task<List<RedditPost>> GetData(string subreddit)
        {
            string uri = $"{baseUrl}/r/{subreddit}/new.json?before=1m";
            
            string token = await GetToken();
            HttpResponseMessage response = await _httpClientWrapper.GetAsync($"{baseUrl}/{subreddit}/top.json", new AuthenticationHeaderValue("Bearer", token));

            ResponseHeader = _utilityService.GetResponseHeader(response);
            
            string jsonContent = await response.Content.ReadAsStringAsync();
            var redditPosts = JsonConvert.DeserializeObject<RedditPostList>(jsonContent).Data.Children;

            return redditPosts;
        }        
    }
}
