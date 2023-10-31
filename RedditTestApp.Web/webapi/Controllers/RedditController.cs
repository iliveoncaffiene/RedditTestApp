using Microsoft.AspNetCore.Mvc;

namespace RedditTestApp.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class RedditController : ControllerBase
{
    private readonly IRedditPostDataStore dataStore;

    public RedditController(IRedditPostDataStore dataStore)
    {
        this.dataStore = dataStore;
    }

    [HttpGet("{subreddit?}")]
    public object Get(string? subreddit = null)
    {
        var posts = this.dataStore.QueryPosts(subreddit);

        return new
        {
            PostsByUpvotes = posts.OrderByDescending(x => x.Upvotes).Take(10).ToList(),
            UsersWithPosts = posts.GroupBy(x => x.Author).Select(x => new { Author = x.Key, Posts = x.Count() }).OrderByDescending(x => x.Posts).Take(10).ToList(),
        };
    }
}
