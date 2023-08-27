using System.Net;
using System.Net.Http.Headers;
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
        public async Task GetDataAsync_ReturnsNewPost()
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
                    Content = new StringContent(Content())
                });

            var apiService = new ApiService(_configuration, _utilityService, _httpClientWrapperMock.Object);

            // Act
            var result = await apiService.GetData("gaming");
            
            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result[0].Data);
            Assert.IsTrue(result[0].Data.Subreddit == "gaming");
            Assert.IsTrue(result[0].Data.CommentsCount == 3373);
            Assert.IsTrue(result[0].Data.Author == "brotherlymoses");
            Assert.IsTrue(result[0].Data.Ups == 30428);
            Assert.IsTrue(result[0].Data.Created == 1692904875.0m);
            Assert.IsTrue(result[0].Data.Title == "Easy choice");

            Assert.NotNull(result[1].Data);
            Assert.IsTrue(result[1].Data.Subreddit == "gaming");
            Assert.IsTrue(result[1].Data.CommentsCount == 166);
            Assert.IsTrue(result[1].Data.Author == "CumboJumbo");
            Assert.IsTrue(result[1].Data.Ups == 10058);
            Assert.IsTrue(result[1].Data.Created == 1692957025.0m);
            Assert.IsTrue(result[1].Data.Title == "When you find and equip a labcoat while playing a Charisma build");
        }

        [Test]
        public async Task GetDataAsync_ReturnsNullWhenNoPosts()
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
            var result = await apiService.GetData("yourSubreddit");

            // Assert
            Assert.NotNull(result);
            Assert.NotNull(result.Count == 0);
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

        private string Content() {
            return "{\r\n    \"kind\": \"Listing\",\r\n    \"data\": {\r\n        \"after\": \"t3_160fvv5\",\r\n        \"dist\": 25,\r\n        \"modhash\": null,\r\n        \"geo_filter\": \"\",\r\n        \"children\": [\r\n            {\r\n                \"kind\": \"t3\",\r\n                \"data\": {\r\n                    \"subreddit\": \"gaming\",\r\n                    \"title\": \"Easy choice\",\r\n                    \"ups\": 30428,\r\n                    \"author\": \"brotherlymoses\",\r\n                    \"num_comments\": 3373,\r\n                    \"created\": 1692904875.0\r\n                }\r\n            },\r\n            {\r\n                \"kind\": \"t3\",\r\n                \"data\": {\r\n                    \"subreddit\": \"gaming\",\r\n                    \"title\": \"When you find and equip a labcoat while playing a Charisma build\",\r\n                    \"ups\": 10058,\r\n                    \"author\": \"CumboJumbo\",\r\n                    \"num_comments\": 166,\r\n                    \"created\": 1692957025.0\r\n                }\r\n            }           \r\n        ],\r\n        \"before\": null\r\n    }\r\n}";
        }
    }
}