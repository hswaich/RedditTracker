using RedditTracker.Repository;
using RedditTracker.Structures;

namespace RedditTracker.Services
{
    public class WatcherService : IWatcherService
    {
        private readonly IApiService _apiService;
        private readonly IUtilityService _utilityService;
        private readonly IDatabaseRepository _databaseRepository; 
        private List<string> _subreddits;
        private bool _databaseTableIsValid;
                
        public WatcherService(IApiService apiService, IUtilityService utilityService, IDatabaseRepository databaseRepository)
        {
            _apiService = apiService;
            _utilityService = utilityService;
            _databaseRepository = databaseRepository;            
        }

        public async Task Run(List<string> subreddits)
        {
            _subreddits = subreddits;
            _databaseTableIsValid = await _databaseRepository.CheckDatabaseConnectionAndTable();
            if (_databaseTableIsValid) 
            {
                Console.WriteLine("Database connection successful.");
            }
            
            await Run();
        }

        private async Task Run()
        {
            int delay = 0;
            List<Task<ISubreddit>> taskList = new List<Task<ISubreddit>>(); // sr times 2 e.g. 3 sr = 6 tasks
            foreach (var subreddit in _subreddits)
            {
                taskList.Add(_apiService.GetTopPostWithMostUpvotesAsync(subreddit));
                taskList.Add(_apiService.GetUserWithMostPostsAsync(subreddit));
            }
            
            try
            {
                ISubreddit[] results = await Task.WhenAll(taskList);
                delay = _utilityService.CalculateDelay(_apiService.ResponseHeader, taskList.Count);

                List<SubredditMessage> subredditMessages = new List<SubredditMessage>();
                
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

                    subredditMessages.Add(new SubredditMessage { Subreddit = subreddit, Message = printLine });
                    Console.WriteLine(printLine);
                    counter++;
                }

                //insert to db all records at once
                if (_databaseTableIsValid && subredditMessages.Count > 0)
                {
                    await _databaseRepository.Insert(subredditMessages);
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
