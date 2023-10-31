using RedditTestApp.RedditClient.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.RateLimiting;
using System.Threading.Tasks;

namespace RedditTestApp.RedditClient;

public class RedditClient : IRedditClient, IDisposable
{
    private readonly IRedditAuthService _authService;
    private readonly RateLimiter _rateLimiter;
    private readonly HttpClient _httpClient;

    private static TimeSpan AuthorizationBufferTime = TimeSpan.FromMinutes(5);

    public RedditClient(IRedditAuthService authService, RateLimiter rateLimiter)
    {
        _authService = authService;
        _rateLimiter = rateLimiter;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://oauth.reddit.com"),
            DefaultRequestHeaders =
            {
                UserAgent =
                {
                    new ProductInfoHeaderValue("RedditTestApp-ASPNETCORE-WebApp", "v1.0")
                }
            }
        };
    }

    private RedditOAuthResponseModel OAuthCredentialsModel { get; set; }

    private DateTime? LastAuthTime { get; set; }

    private DateTime? TokenExpiration => OAuthCredentialsModel != null ? LastAuthTime?.Add(OAuthCredentialsModel.ExpirationDuration) : null;

    private async Task EnsureAuthorization()
    {
        if (DateTime.UtcNow.Add(AuthorizationBufferTime) < TokenExpiration)
        {
            return;
        }

        try
        {
            var response = await _authService.Authorize();
            LastAuthTime = DateTime.UtcNow;
            OAuthCredentialsModel = response;
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", response.AccessToken);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine(ex.StackTrace);
            LastAuthTime = null;
            OAuthCredentialsModel = null;
        }
    }

    public async Task<RedditResponseModel<RedditListing<RedditPost>>> GetPostsBySubreddit(string subreddit, int count = 100, string sort = "new", string before = null, string after = null)
    {
        await EnsureAuthorization();

        var queryString = new Dictionary<string, string>
        {
            ["count"] = count.ToString(),
            ["before"] = before,
            ["after"] = after,
        }.Where(kv => kv.Value != null).Select(kv => $"{kv.Key}={kv.Value}");

        HttpResponseMessage response;
        using (var lease = await _rateLimiter.AcquireAsync())
        {
            response = await _httpClient.GetAsync($"/r/{subreddit}/{sort}?{string.Join("&", queryString)}");
        }
        var responseModel = await response.Content.ReadFromJsonAsync<RedditResponseModel<RedditListing<RedditPost>>>();

        return responseModel;
    }

    public async Task<IEnumerable<RedditPost>> GetUpdatedPostsByPostIds(IEnumerable<string> postIds)
    {
        const int maxIdsPerQuery = 20;

        var batchedIds = postIds.Select((id, i) => new { id, i }).GroupBy(x => x.i / maxIdsPerQuery, x => x.id).ToList();

        var list = new List<RedditPost>();

        HttpResponseMessage[] responses;
        using (var lease = await _rateLimiter.AcquireAsync(batchedIds.Count))
        {
            responses = await Task.WhenAll(batchedIds.Select(idg =>
            {
                var response = _httpClient.GetAsync($"/by_id/{string.Join(",", idg)}");

                return response.ContinueWith(r => r.Result.StatusCode == System.Net.HttpStatusCode.OK ? r.Result : null);
            }));
        }
        var jsonResponses = await Task.WhenAll(responses.Where(r => r != null).Select(r => r.Content.ReadFromJsonAsync<RedditResponseModel<RedditListing<RedditPost>>>()));

        foreach (var response in jsonResponses)
        {
            list.AddRange(response.Data.Children.Select(c => c.Data));
        }

        return list;
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
