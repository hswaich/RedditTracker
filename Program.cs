using Microsoft.Extensions.DependencyInjection;
using RedditTracker.Services;
using RedditTracker;

var services = Startup.ConfigureServices();
var serviceProvider = services.BuildServiceProvider();

await serviceProvider.GetService<IWatcherService>().Run();