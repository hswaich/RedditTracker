using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using RedditTracker.Services;
using RedditTracker.Structures;

namespace RedditTracker.Test
{
    [TestFixture]
    public class ApiServiceTests
    {
        private IUtilityService _utilityService;
        private IConfiguration _configuration;
        private Mock<IHttpClientWrapper> _httpClientWrapperMock;
        private AccessTokenResponse _accessTokenResponse;


        [SetUp]
        public void Setup()
        {
            _configuration = new ConfigurationBuilder()
               .AddInMemoryCollection(new[]
               {
                    new KeyValuePair<string, string>("BaseUrl", "MockedValue"),
                    new KeyValuePair<string, string>("AccessTokenUrl", "MockedValue"),
               })
               .Build();

            _utilityService = new UtilityService();
            _httpClientWrapperMock = new Mock<IHttpClientWrapper>();
        }

        [Test]
        public async Task GetToken_ReturnsAccessToken()
        {
            // Arrange
            SetMockTokenData();
            var apiService = new ApiService(_configuration, _utilityService, _httpClientWrapperMock.Object);

            // Act
            var result = await apiService.GetToken();

            // Assert
            Assert.IsTrue(_accessTokenResponse.access_token == result);
        }

        [Test]
        public async Task GetToken_ReturnsAccessToken_Failure()
        {
            // Arrange
            _httpClientWrapperMock.Setup(wrapper => wrapper.PostAsync(
                It.IsAny<string>(),
                It.IsAny<FormUrlEncodedContent>(),
                It.IsAny<AuthenticationHeaderValue>())
                ).ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("some content error")
                });

            var apiService = new ApiService(_configuration, _utilityService, _httpClientWrapperMock.Object);

            // Act
            var result = await apiService.GetToken();

            // Assert
            Assert.IsTrue(string.Empty == result);
        }

        [Test]
        public async Task GetTopPostWithMostUpvotesAsync_ReturnsTopPost()
        {
            // Arrange
            SetMockTokenData();
            _httpClientWrapperMock
                .Setup(wrapper => wrapper.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<AuthenticationHeaderValue>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"data\": {\"children\": [{\"data\": {\"ups\": 5,\"title\": \"title1\",\"url\": \"\"}},{\"data\": {\"ups\": 15, \"title\": \"title2\", \"url\": \"\" }},{\"data\": {\"ups\": 3,\"title\": \"title3\",\"url\": \"\"}}]}}")
                });

            var apiService = new ApiService(_configuration, _utilityService, _httpClientWrapperMock.Object);

            // Act
            var result = await apiService.GetTopPostWithMostUpvotesAsync("yourSubreddit");

            // Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.Subreddit == "yourSubreddit");
            Assert.IsTrue(((TopTitleUpvote)result).Title == "title2");
            Assert.IsTrue(((TopTitleUpvote)result).Upvotes == 15);
        }

        [Test]
        public async Task GetTopPostWithMostUpvotesAsync_ReturnsNullWhenNoPosts()
        {
            // Arrange
            SetMockTokenData();
            _httpClientWrapperMock
                .Setup(wrapper => wrapper.GetAsync(It.IsAny<string>(), It.IsAny<AuthenticationHeaderValue>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"data\": {\"children\": []}}")
                });

            var apiService = new ApiService(_configuration, _utilityService, _httpClientWrapperMock.Object);

            // Act
            var result = await apiService.GetTopPostWithMostUpvotesAsync("yourSubreddit");

            // Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.Subreddit == "yourSubreddit");
            Assert.IsTrue(((TopTitleUpvote)result).Title == null);
            Assert.IsTrue(((TopTitleUpvote)result).Upvotes == 0);
        }

        [Test]
        public async Task GetUserWithMostPostsAsync_ReturnsTopAuthor()
        {
            // Arrange
            SetMockTokenData();
            _httpClientWrapperMock
                .Setup(wrapper => wrapper.GetAsync(
                    It.IsAny<string>(),
                    It.IsAny<AuthenticationHeaderValue>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"data\": {\"children\": [{\"data\": {\"title\": \"title1\", \"author\": \"james\"}},{\"data\": {\"title\": \"title2\", \"author\": \"james\" }},{\"data\": {\"title\": \"title3\", \"author\": \"mary\"}}]}}")
                });

            var apiService = new ApiService(_configuration, _utilityService, _httpClientWrapperMock.Object);

            // Act
            var result = await apiService.GetUserWithMostPostsAsync("yourSubreddit");

            // Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.Subreddit == "yourSubreddit");
            Assert.IsTrue(((TopUser)result).User == "james");
        }

        [Test]
        public async Task GetUserWithMostPostsAsync_ReturnsNullWhenNoTopAuthor()
        {
            // Arrange
            SetMockTokenData();
            _httpClientWrapperMock
                .Setup(wrapper => wrapper.GetAsync(It.IsAny<string>(), It.IsAny<AuthenticationHeaderValue>()))
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"data\": {\"children\": []}}")
                });

            var apiService = new ApiService(_configuration, _utilityService, _httpClientWrapperMock.Object);

            // Act
            var result = await apiService.GetUserWithMostPostsAsync("yourSubreddit");

            // Assert
            Assert.NotNull(result);
            Assert.IsTrue(result.Subreddit == "yourSubreddit");
            Assert.IsTrue(((TopUser)result).User == String.Empty);
        }

        private void SetMockTokenData()
        {
            _accessTokenResponse = new AccessTokenResponse
            {
                access_token = "validAccessToken",
                expires_in = 3600 // 1 hour
            };

            _httpClientWrapperMock.Setup(wrapper => wrapper.PostAsync(
                It.IsAny<string>(),
                It.IsAny<FormUrlEncodedContent>(),
                It.IsAny<AuthenticationHeaderValue>())
            ).ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JObject.FromObject(_accessTokenResponse).ToString())
            });
        }
    }
}