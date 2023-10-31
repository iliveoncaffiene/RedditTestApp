using System;
using System.Text.Json.Serialization;

namespace RedditTestApp.RedditClient.Models;

public class RedditOAuthResponseModel
{
    [JsonPropertyName("access_token")]
    public string AccessToken { get; set; }

    [JsonPropertyName("token_type")]
    public string TokenType { get; set; }

    [JsonPropertyName("expires_in")]
    public int ExpiresIn { get; set; }

    public TimeSpan ExpirationDuration => TimeSpan.FromSeconds(ExpiresIn);

    [JsonPropertyName("scope")]
    public string Scope { get; set; }
}
