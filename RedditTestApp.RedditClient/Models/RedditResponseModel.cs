using System.Text.Json.Serialization;

namespace RedditTestApp.RedditClient.Models;

public class RedditResponseModel<TData>
{
    [JsonPropertyName("kind")]
    public string ResponseType { get; set; }

    [JsonPropertyName("data")]
    public TData Data { get; set; }
}