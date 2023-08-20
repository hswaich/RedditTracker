using RedditTracker.Structures;

namespace RedditTracker.Services
{
    public class WatcherService : IWatcherService
    {
        private readonly IApiService _apiService;
        private readonly IUtilityService _utilityService;

        public WatcherService(IApiService apiService, IUtilityService utilityService)
        {
            _apiService = apiService;
            _utilityService = utilityService;
        }

        public async Task Run()
        {
            int delay = 0;

            List<string> subreddits = new List<string>();
            subreddits.Add("funny");
            subreddits.Add("gaming");
            subreddits.Add("aww");


            List<Task<ISubreddit>> taskList = new List<Task<ISubreddit>>();
            foreach (var subreddit in subreddits)
            {
                taskList.Add(_apiService.GetTopPostWithMostUpvotesAsync(subreddit));
                taskList.Add(_apiService.GetUserWithMostPostsAsync(subreddit));
            }

            try
            {
                ISubreddit[] results = await Task.WhenAll(taskList);
                delay = _utilityService.CalculateDelay(_apiService.ResponseHeader, taskList.Count);

                int counter = 1;
                foreach (string subreddit in subreddits)
                {
                    var subredditResult = results.Where(x => x.Subreddit == subreddit).ToList();

                    string printLine = _utilityService.PrintLine(subredditResult);

                    if (counter == subreddits.Count)
                    {
                        printLine = printLine + " " + _utilityService.PrintResponseHeaderStatus(_apiService.ResponseHeader, delay);
                    }

                    Console.WriteLine(printLine);
                    counter++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
            }

            Thread.Sleep(delay);

            await Run();
        }
    }
}
