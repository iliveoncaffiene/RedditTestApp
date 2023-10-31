using RedditTestApp.RedditClient.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RedditTestApp.RedditClient;

public interface IRedditClient
{
    Task<RedditResponseModel<RedditListing<RedditPost>>> GetPostsBySubreddit(string subreddit, int count = 100, string sort = "new", string before = null, string after = null);

    Task<IEnumerable<RedditPost>> GetUpdatedPostsByPostIds(IEnumerable<string> postIds);
}