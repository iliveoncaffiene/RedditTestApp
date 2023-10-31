using Microsoft.Extensions.DependencyInjection;

namespace RedditTestApp.Worker;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSubredditMonitor(this IServiceCollection services, ICollection<string> subreddits)
    {
        return services.AddHostedService(provider => new PollRedditPostsHostedService(provider, subreddits));
    }
}
