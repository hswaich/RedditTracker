
using NUnit.Framework;
using RedditTracker.Services;
using RedditTracker.Structures;

namespace RedditTracker.Test
{
    [TestFixture]
    public class UtilityServiceTests
    {
        private IUtilityService _utilityService;

        [SetUp]
        public void SetUp()
        {
            _utilityService = new UtilityService();
        }

        [Test]
        public void PrintResponseHeaderStatus_WhenCalled_ReturnsFormattedString()
        {
            // Arrange
            var responseHeader = new ResponseHeader
            {
                RateLimitRemaining = 100,
                RateLimitUsed = 200,
                RateLimitReset = 300
            };
            var delay = 5000;

            // Act
            var result = _utilityService.PrintResponseHeaderStatus(responseHeader, delay);

            // Assert
            Assert.IsTrue(result == "100/200 RMNG/used calls. 300/5 reset/delay secs");
        }

        //[Test]
        //public void CalculateDelay_WhenCalledWithNullResponseHeaderAndTaskCount_ReturnsDelay()
        //{
        //    // Arrange
        //    var taskCount = 2;

        //    // Act
        //    var result = _utilityService.CalculateDelay(null, taskCount);

        //    // Assert
        //    Assert.AreEqual(0, result);
        //}

        //[Test]
        //public void CalculateDelay_WhenCalledWithResponseHeaderAndTaskCount_ReturnsDelay()
        //{
        //    // Arrange
        //    var responseHeader = new ResponseHeader
        //    {
        //        RateLimitRemaining = 100,
        //        RateLimitReset = 300
        //    };
        //    var taskCount = 2;

        //    // Act
        //    var result = _utilityService.CalculateDelay(responseHeader, taskCount);

        //    // Assert
        //    Assert.That(result, Is.EqualTo(6000));
        //}

        //[Test]
        //public void CalculateDelay_WhenCalledWithResponseHeaderAnd_TaskCountExceedsLimitRemaining_ReturnsDelay()
        //{
        //    // Arrange
        //    var responseHeader = new ResponseHeader
        //    {
        //        RateLimitRemaining = 2,
        //        RateLimitReset = 300
        //    };
        //    var taskCount = 5;

        //    // Act
        //    var result = _utilityService.CalculateDelay(responseHeader, taskCount);

        //    // Assert
        //    Assert.That(result, Is.EqualTo(300));
        //}
    }
}