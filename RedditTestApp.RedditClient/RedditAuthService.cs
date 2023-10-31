using Microsoft.Extensions.Options;
using RedditTestApp.RedditClient.Models;
using RedditTestApp.RedditClient.Options;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace RedditTestApp.RedditClient;

public class RedditAuthService : IRedditAuthService
{
    private const string RedditAuthEndpoint = "/api/v1/access_token";

    private readonly IOptions<RedditOptions> _options;

    public RedditAuthService(IOptions<RedditOptions> options)
    {
        _options = options;
    }

    public async Task<RedditOAuthResponseModel> Authorize()
    {
        using var client = new HttpClient { BaseAddress = new Uri("https://www.reddit.com") };

        //Post body content
        var values = new List<KeyValuePair<string, string>>
        {
            new KeyValuePair<string, string>("grant_type", "client_credentials")
        };
        var content = new FormUrlEncodedContent(values);

        var authenticationString = $"{_options.Value.ClientId}:{_options.Value.Secret}";
        var base64EncodedAuthenticationString = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes(authenticationString));

        var requestMessage = new HttpRequestMessage(HttpMethod.Post, RedditAuthEndpoint);
        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64EncodedAuthenticationString);
        requestMessage.Headers.UserAgent.Add(new ProductInfoHeaderValue("RedditTestApp-ASPNETCORE-WebApp", "v1.0"));
        requestMessage.Content = content;
        var response = await client.SendAsync(requestMessage);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<RedditOAuthResponseModel>();
    }
}

