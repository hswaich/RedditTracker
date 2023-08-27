using NUnit.Framework;
using RedditTracker.Services;
using RedditTracker.Structures;

namespace RedditTracker.Test
{
    [TestFixture]
    public class StatisticsServiceTests
    {
        private IStatisticsService _statisticsService;
        
        [SetUp]
        public void Setup()
        {
            _statisticsService = new StatisticsService();
        }

        [Test]
        public async Task Process_IsInitialDataSetupCorrectly()
        {
            // Act
            await _statisticsService.Process(GetRedditPosts());
            var subreddit1 = _statisticsService.Statistics[0];
         
            //// Assert
            Assert.IsTrue(subreddit1.InitialAuthorPosts.Count == 3);
            Assert.IsTrue(subreddit1.InitialTitleUpvotesCount.Count == 4);

            Assert.IsTrue(subreddit1.InitialAuthorPosts["Author 1"].Count() == 2);
            Assert.IsTrue(subreddit1.InitialAuthorPosts["Author 2"].Count() == 1);
            Assert.IsTrue(subreddit1.InitialAuthorPosts["Author 4"].Count() == 1);

            Assert.IsTrue(subreddit1.InitialTitleUpvotesCount["Title 1"] == 10);
            Assert.IsTrue(subreddit1.InitialTitleUpvotesCount["Title 2"] == 20);
            Assert.IsTrue(subreddit1.InitialTitleUpvotesCount["Title 3"] == 30);
            Assert.IsTrue(subreddit1.InitialTitleUpvotesCount["Title 4"] == 40);
        }

        [Test]
        public async Task Calculation_IsCalculateDataSetupCorrectly()
        {
            // Act
            await _statisticsService.Process(GetRedditPosts());
            var subreddit1 = _statisticsService.Statistics[0];

            //// Assert
            Assert.IsTrue(subreddit1.AuthorPosts.Count == 0);
            Assert.IsTrue(subreddit1.TitleUpvotesCount.Count == 4);
     
            Assert.IsTrue(subreddit1.TitleUpvotesCount["Title 1"] == 0);
            Assert.IsTrue(subreddit1.TitleUpvotesCount["Title 2"] == 0);
            Assert.IsTrue(subreddit1.TitleUpvotesCount["Title 3"] == 0);
            Assert.IsTrue(subreddit1.TitleUpvotesCount["Title 4"] == 0);
        }

        [Test]
        public async Task Calculation_IsCalculateDataCalculatesUpdatesCorrectly()
        {
            // Act
            await _statisticsService.Process(GetRedditPosts()); //initial
            await _statisticsService.Process(GetRedditPostsUpdates()); //updates
            
            var subreddit1 = _statisticsService.Statistics[0];

            //// Assert
            Assert.IsTrue(subreddit1.AuthorPosts.Count == 2);
            Assert.IsTrue(subreddit1.TitleUpvotesCount.Count == 6);

            Assert.IsTrue(subreddit1.AuthorPosts["Author 1"].Count() == 1);
            Assert.IsTrue(subreddit1.AuthorPosts["Author 2"].Count() == 1);
            Assert.IsFalse(subreddit1.AuthorPosts.ContainsKey("Author 3"));
            Assert.IsFalse(subreddit1.AuthorPosts.ContainsKey("Author 4"));

            Assert.IsTrue(subreddit1.TitleUpvotesCount["Title 1"] == 5);
            Assert.IsTrue(subreddit1.TitleUpvotesCount["Title 2"] == 0);
            Assert.IsTrue(subreddit1.TitleUpvotesCount["Title 3"] == 7);
            Assert.IsTrue(subreddit1.TitleUpvotesCount["Title 4"] == 0);
            Assert.IsTrue(subreddit1.TitleUpvotesCount["Title 5"] == 0);
            Assert.IsTrue(subreddit1.TitleUpvotesCount["Title 6"] == 0);
        }

        private List<RedditPostData> GetRedditPosts() 
        {
            return new List<RedditPostData> 
            {
                new RedditPostData{ Title = "Title 1", Author = "Author 1", Subreddit = "Subreddit 1", Ups = 10 },
                new RedditPostData{ Title = "Title 2", Author = "Author 2", Subreddit = "Subreddit 1", Ups = 20 },
                new RedditPostData{ Title = "Title 3", Author = "Author 1", Subreddit = "Subreddit 1", Ups = 30 },
                new RedditPostData{ Title = "Title 4", Author = "Author 4", Subreddit = "Subreddit 1", Ups = 40 },
            };
        }

        private List<RedditPostData> GetRedditPostsUpdates()
        {
            return new List<RedditPostData>
            {
                new RedditPostData{ Title = "Title 1", Author = "Author 1", Subreddit = "Subreddit 1", Ups = 15 },
                new RedditPostData{ Title = "Title 5", Author = "Author 2", Subreddit = "Subreddit 1", Ups = 23 },
                new RedditPostData{ Title = "Title 3", Author = "Author 1", Subreddit = "Subreddit 1", Ups = 37 },
                new RedditPostData{ Title = "Title 6", Author = "Author 1", Subreddit = "Subreddit 1", Ups = 90 },
            };
        }
    }
}