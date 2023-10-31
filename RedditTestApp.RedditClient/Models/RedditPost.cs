using System.Text.Json.Serialization;

namespace RedditTestApp.RedditClient.Models;

public class RedditPost
{
    [JsonPropertyName("author_fullname")]
    public string AuthorFullname { get; set; }

    [JsonPropertyName("author")]
    public string Author { get; set; }

    [JsonPropertyName("name")]
    public string Fullname { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("subreddit")]
    public string Subreddit { get; set; }

    [JsonPropertyName("ups")]
    public int Upvotes { get; set; }

    [JsonPropertyName("score")]
    public int Score { get; set; }

    [JsonPropertyName("clicked")]
    public bool Clicked { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}
