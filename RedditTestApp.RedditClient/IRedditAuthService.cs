using RedditTestApp.RedditClient.Models;
using System.Threading.Tasks;

namespace RedditTestApp.RedditClient;

public interface IRedditAuthService
{
    Task<RedditOAuthResponseModel> Authorize();
}
