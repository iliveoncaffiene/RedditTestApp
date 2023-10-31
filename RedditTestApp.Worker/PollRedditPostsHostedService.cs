using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RedditTestApp.RedditClient;
using RedditTestApp.RedditClient.Models;
using System.Diagnostics;

namespace RedditTestApp.Worker;

public class PollRedditPostsHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ICollection<string> _subreddits;

    public PollRedditPostsHostedService(IServiceProvider serviceProvider, ICollection<string> subreddits)
    {
        _serviceProvider = serviceProvider;
        _subreddits = subreddits;
    }

    private string? NewestPostFullName { get; set; }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        return Task.WhenAll(_subreddits.Select(x => SubredditListenAsync(stoppingToken, x)));
    }

    public async Task SubredditListenAsync(CancellationToken token, string subreddit)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(20));

        while (await timer.WaitForNextTickAsync(token))
        {
            using var scope = _serviceProvider.CreateScope();

            var client = scope.ServiceProvider.GetRequiredService<IRedditClient>();
            var dataStore = scope.ServiceProvider.GetRequiredService<IWorkerDataStore<RedditPost>>();

            var posts = await client.GetPostsBySubreddit(subreddit, count: 25, before: NewestPostFullName);
            if (posts.Data.Children.Count > 0)
            {
                NewestPostFullName = posts.Data.Children.First().Data.Fullname;
                Debug.WriteLine($"Found {posts.Data.Children.Count} new posts in subreddit {subreddit}");
                dataStore.InsertOrUpdate(posts.Data.Children.Select(c => c.Data));
            }
        }
    }
}