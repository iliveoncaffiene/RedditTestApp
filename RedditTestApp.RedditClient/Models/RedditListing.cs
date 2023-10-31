using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RedditTestApp.RedditClient.Models;

public class RedditListing<TChild>
{
    [JsonPropertyName("after")]
    public string After { get; set; }

    [JsonPropertyName("before")]
    public string Before { get; set; }

    [JsonPropertyName("children")]
    public List<RedditResponseModel<TChild>> Children { get; set; }
}
