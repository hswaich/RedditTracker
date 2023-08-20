using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RedditTracker.Structures;
using System.Net.Http.Headers;
using System.Text;

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
            string? v = configuration.GetValue<string>("BaseUrl");
            baseUrl = v;
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
            //httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            //  HttpResponseMessage authResponse = await httpClient.PostAsync(accessTokenUrl, authData);
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

        public async Task<ISubreddit> GetTopPostWithMostUpvotesAsync(string subreddit)
        {
            string token = await GetToken();
            // httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // HttpResponseMessage response = await httpClient.GetAsync($"{baseUrl}/{subreddit}/top.json");
            HttpResponseMessage response = await _httpClientWrapper.GetAsync($"{baseUrl}/{subreddit}/top.json", new AuthenticationHeaderValue("Bearer", token));
            string jsonContent = await response.Content.ReadAsStringAsync();

            var posts = JsonConvert.DeserializeObject<JObject>(jsonContent)["data"]["children"];

            ResponseHeader = _utilityService.GetResponseHeader(response);

            Post topPost = new Post();
            int maxUpvotes = 0;

            foreach (var post in posts)
            {
                int upvotes = post["data"]["ups"].Value<int>();
                if (upvotes > maxUpvotes)
                {
                    maxUpvotes = upvotes;
                    topPost = post["data"].ToObject<Post>();
                }
            }

            return new TopTitleUpvote(topPost.Title, topPost.Upvotes, subreddit);
        }

        public async Task<ISubreddit> GetUserWithMostPostsAsync(string subreddit)
        {
            string token = await GetToken();
            //   httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // HttpResponseMessage response = await httpClient.GetAsync($"{baseUrl}/{subreddit}/new.json?limit=10000");
            HttpResponseMessage response = await _httpClientWrapper.GetAsync($"{baseUrl}/{subreddit}/new.json?limit=10000", new AuthenticationHeaderValue("Bearer", token));

            string jsonContent = await response.Content.ReadAsStringAsync();
            var posts = JsonConvert.DeserializeObject<JObject>(jsonContent)["data"]["children"];

            var userPostCounts = new Dictionary<string, int>();
            ResponseHeader = _utilityService.GetResponseHeader(response);

            foreach (var post in posts)
            {
                string author = post["data"]["author"].Value<string>();
                if (!string.IsNullOrEmpty(author))
                {
                    if (userPostCounts.ContainsKey(author))
                    {
                        userPostCounts[author]++;
                    }
                    else
                    {
                        userPostCounts[author] = 1;
                    }
                }
            }

            int maxPostCount = 0;
            string topUser = String.Empty;

            if (userPostCounts.Count > 0)
            {
                maxPostCount = userPostCounts.Select(a => a.Value).Max();
                topUser = userPostCounts.Where(a => a.Value == maxPostCount).Select(a => a.Key).FirstOrDefault();
            }

            return new TopUser(topUser, subreddit);
        }
    }
}
