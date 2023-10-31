using RedditTestApp.RedditClient;

namespace RedditTestApp.Api;

public class UpdateRedditPostsHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;

    public UpdateRedditPostsHostedService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(5));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            await UpdatePosts();
        }
    }

    public async Task UpdatePosts()
    {
        using var scope = _serviceProvider.CreateScope();

        var client = scope.ServiceProvider.GetRequiredService<IRedditClient>();
        var dataStore = scope.ServiceProvider.GetRequiredService<IRedditPostDataStore>();
        var postIds = dataStore.QueryPosts().Select(p => p.Fullname).Distinct();

        var posts = await client.GetUpdatedPostsByPostIds(postIds);
        dataStore.InsertOrUpdate(posts);
    }
}