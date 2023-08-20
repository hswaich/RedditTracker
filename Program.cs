using Microsoft.Extensions.DependencyInjection;
using RedditTracker.Services;
using RedditTracker;

var services = Startup.ConfigureServices();
var serviceProvider = services.BuildServiceProvider();

List<string> subreddits = new List<string>();
subreddits.Add("funny");
subreddits.Add("gaming");
subreddits.Add("aww");

await serviceProvider.GetService<IWatcherService>().Run(subreddits);