using RedditTestApp.RedditClient.Models;
using RedditTestApp.Worker;
using System.Collections.Concurrent;

namespace RedditTestApp.Api;

public class RedditPostDataStore : IRedditPostDataStore
{
    private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, RedditPost>> _posts;

    public RedditPostDataStore()
    {
        _posts = new();
    }

    void IWorkerDataStore<RedditPost>.InsertOrUpdate(IEnumerable<RedditPost> data)
    {
        foreach (var groupedPosts in data.GroupBy(x => x.Subreddit))
        {
            _posts.AddOrUpdate(groupedPosts.Key,
                key => new ConcurrentDictionary<string, RedditPost>(groupedPosts.ToDictionary(x => x.Fullname)),
                (key, existing) =>
                {
                    foreach (var post in groupedPosts)
                    {
                        existing.AddOrUpdate(post.Fullname, post,
                            (key, old) => post);
                    }

                    return existing;
                });
        }
    }

    public IQueryable<RedditPost> QueryPosts(string? subreddit = null)
    {
        if (subreddit == null)
        {
            return _posts.SelectMany(x => x.Value.Values).AsQueryable();
        }

        return _posts.TryGetValue(subreddit, out var posts) ? posts.Values.AsQueryable() : Enumerable.Empty<RedditPost>().AsQueryable();
    }
}
