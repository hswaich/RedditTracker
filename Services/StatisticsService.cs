using RedditTracker.Structures;
using System.Text;

namespace RedditTracker.Services
{
    public class StatisticsService : IStatisticsService
    {
        public StatisticsService()
        {
            Statistics = new List<Statistic>();
        }        

        public List<Statistic> Statistics { get; set; }

        public async Task<bool> Process(List<RedditPostData> list)
        {
            //Add subReddit to Statistics
            var subReddit = list.FirstOrDefault()?.Subreddit;

            if (!Statistics.Any(a => a.Subreddit == subReddit))
            {
                Statistics.Add(new Statistic(subReddit));
            }

            var statistic = Statistics.FirstOrDefault(a => a.Subreddit == subReddit);

            // Add to inital Upvote Data
            foreach (var post in list)
            {
                if (!statistic.InitialTitleUpvotesCount.ContainsKey(post.Title))
                {
                    statistic.InitialTitleUpvotesCount.Add(post.Title, post.Ups);
                }
            }

            if (!statistic.isAuthorPostsInitialized)
            {
                // Add to inital Author Data
                foreach (var author in list.Select(a => a.Author).Distinct())
                {
                    var posts = list.Where(a => a.Author == author).Select(a => a.Title).ToList();
                    statistic.InitialAuthorPosts.Add(author, posts);
                }
                statistic.isAuthorPostsInitialized = true;
            }
            

            //Calulate TitleUpvotesCount
            foreach (var post in list)
            {
                if (statistic.TitleUpvotesCount.ContainsKey(post.Title))
                {
                    statistic.TitleUpvotesCount[post.Title] = post.Ups - statistic.InitialTitleUpvotesCount[post.Title];
                }
                else
                {
                    statistic.TitleUpvotesCount.Add(post.Title, post.Ups - statistic.InitialTitleUpvotesCount[post.Title]);
                }
            }

            //Calulate AuthorPostCount
            foreach (var author in list.Select(a => a.Author).Distinct())
            {

                var initialPosts = statistic.InitialAuthorPosts[author].Select(a => a).ToList();
                var currentPosts = list.Where(a => a.Author == author).Select(a => a.Title).ToList();
                var newPosts = currentPosts.Except(initialPosts).ToList();

                if (newPosts.Count > 0)
                {
                    if (statistic.AuthorPosts.ContainsKey(author))
                    {
                        statistic.AuthorPosts[author].AddRange(newPosts);
                    }
                    else
                    {
                        statistic.AuthorPosts.Add(author, newPosts);
                    }
                }                
            }

            //Print or Save to db
            List<string> printArray = new List<string>();

            printArray.Add($"{subReddit}");
            var maxVotes = statistic.TitleUpvotesCount.Max(a => a.Value);
            var maxVotesKey = statistic.TitleUpvotesCount.OrderBy(a => a.Key).First(b => b.Value == maxVotes).Key;

            var authorWithMostPosts = statistic.AuthorPosts.OrderByDescending(kvp => kvp.Value.Count).Select(kvp => kvp.Key).FirstOrDefault();

            if (authorWithMostPosts == null)
            {
                printArray.Add($"No new Author.");
            }
            else
            {
                printArray.Add($"Author: {authorWithMostPosts}.");
            }

            string maxVotesKeytruncatedString = maxVotesKey.Length > 20 ? $"{maxVotesKey.Substring(0, 20)}..." : maxVotesKey;

            printArray.Add($"Most Upvotes {maxVotes} with Title: {maxVotesKeytruncatedString}. ");

            Console.WriteLine(string.Join("|", printArray));


            return await Task.FromResult(true);
        }
    }
}
