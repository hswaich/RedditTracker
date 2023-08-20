using RedditTracker.Structures;

namespace RedditTracker.Services
{
    public class WatcherService : IWatcherService
    {
        private readonly IApiService _apiService;
        private readonly IUtilityService _utilityService;
        private List<string> _subreddits;
        private List<Task<ISubreddit>> taskList;
        
        public WatcherService(IApiService apiService, IUtilityService utilityService)
        {
            _apiService = apiService;
            _utilityService = utilityService;
        }

        public async Task Run(List<string> subreddits)
        {
            _subreddits = subreddits;

            taskList = new List<Task<ISubreddit>>(); // sr times 2 e.g. 3 sr = 6 tasks
            foreach (var subreddit in subreddits)
            {
                taskList.Add(_apiService.GetTopPostWithMostUpvotesAsync(subreddit));
                taskList.Add(_apiService.GetUserWithMostPostsAsync(subreddit));
            }
            await Run();
        }

        private async Task Run()
        {
            int delay = 0;
            
            try
            {
                ISubreddit[] results = await Task.WhenAll(taskList);
                delay = _utilityService.CalculateDelay(_apiService.ResponseHeader, taskList.Count);

                //Printing: consolidate multiple calls for same subReddit on same Line
                int counter = 1;
                foreach (string subreddit in _subreddits)
                {
                    var subredditResult = results.Where(x => x.Subreddit == subreddit).ToList();

                    string printLine = _utilityService.PrintLine(subredditResult);

                    if (counter == _subreddits.Count) // print API stats on lastline
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
