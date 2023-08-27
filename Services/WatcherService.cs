using RedditTracker.Repository;
using RedditTracker.Structures;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace RedditTracker.Services
{
    public class WatcherService : IWatcherService

    {
        private readonly IApiService _apiService;
        private readonly IUtilityService _utilityService;
        private readonly IStatisticsService _statisticsService;
        private List<string> _subreddits;
        private bool _databaseTableIsValid;
        private int subredditCounter = 0;
        ConcurrentQueue<Task<List<RedditPost>>> apiCallTasks;
        ConcurrentQueue<Task<List<RedditPost>>> completedTasks;

        public WatcherService(IApiService apiService, IUtilityService utilityService, IStatisticsService statisticsService)
        {
            _apiService = apiService;
            _utilityService = utilityService;
            _statisticsService = statisticsService;
            apiCallTasks = new ConcurrentQueue<Task<List<RedditPost>>>();
            completedTasks = new ConcurrentQueue<Task<List<RedditPost>>>();
        }

        public async Task Run(List<string> subreddits)
        {
            _subreddits = subreddits;    
            await Run();           
        }

        private async Task Run()
        {
            try
            {
                apiCallTasks.Enqueue(_apiService.GetData(getSubReddit()));

                while (apiCallTasks.Any())
                {
                    var completedTask = await Task.WhenAny(apiCallTasks);

                    if (completedTask.IsCompletedSuccessfully)
                    {
                        //Console.WriteLine($"{(int)_apiService.ResponseHeader.RateLimitRemaining}/{_apiService.ResponseHeader.RateLimitUsed} RMNG/used calls. {_apiService.ResponseHeader.RateLimitReset} reset secs");
                        apiCallTasks.TryDequeue(out completedTask);
                        completedTasks.Enqueue(completedTask);

                        await Task.Delay(_utilityService.GetDelay(_apiService.ResponseHeader));
                        apiCallTasks.Enqueue(_apiService.GetData(getSubReddit())); 
                        ProcessResults(); // no await 
                        
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally 
            {
                apiCallTasks.Clear();
                completedTasks.Clear();
                await Run();
            }
        }

        private async Task ProcessResults()
        {
            //Console.WriteLine($"Processing results...{completedTasks.Count} tasks in queue.");
            //await Task.Delay(10000);
            completedTasks.TryDequeue(out Task<List<RedditPost>> completedTask);
            await _statisticsService.Process(completedTask.Result.Select(a => a.Data).ToList());
            //Console.WriteLine($"Results processed. {completedTasks.Count} tasks in queue..............................");
        }

        private string getSubReddit() 
        {
            string subreddit = _subreddits[subredditCounter];
            subredditCounter++;
            if (subredditCounter == _subreddits.Count)
            {
                subredditCounter = 0;
            }
            return subreddit;
        }
    }
}
