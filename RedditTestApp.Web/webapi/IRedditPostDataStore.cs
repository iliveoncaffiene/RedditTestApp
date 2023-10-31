using RedditTestApp.RedditClient.Models;
using RedditTestApp.Worker;

namespace RedditTestApp.Api;

public interface IRedditPostDataStore : IWorkerDataStore<RedditPost>
{
    IQueryable<RedditPost> QueryPosts(string? subreddit = null);
}
