using RedditTestApp.RedditClient.Models;

namespace RedditTestApp.Api.Tests;

[TestFixture]
public class RedditPostDataStoreFixture
{
    private IRedditPostDataStore dataStore;

    [SetUp]
    public void Setup()
    {
        dataStore = new RedditPostDataStore();
    }

    [Test]
    public void InsertsSuccess()
    {
        dataStore.InsertOrUpdate(new RedditPost[]
        {
            new()
            {
                Id = "1",
                Fullname = "t3_1",
                Subreddit = "sr1"
            },
            new()
            {
                Id = "2",
                Fullname = "t3_2",
                Subreddit = "sr2"
            }
        });

        var allPosts = dataStore.QueryPosts().ToList();
        Assert.That(allPosts, Has.Count.EqualTo(2));

        var sr1Posts = dataStore.QueryPosts("sr1").ToList();
        Assert.That(sr1Posts, Has.Count.EqualTo(1));
        Assert.That(sr1Posts.Single().Id, Is.EqualTo("1"));

        var sr2Posts = dataStore.QueryPosts("sr2").ToList();
        Assert.That(sr2Posts, Has.Count.EqualTo(1));
        Assert.That(sr2Posts.Single().Id, Is.EqualTo("2"));
    }

    [Test]
    public void UpdatesSuccess()
    {
        dataStore.InsertOrUpdate(new RedditPost[]
        {
            new()
            {
                Id = "1",
                Fullname = "t3_1",
                Subreddit = "sr1"
            },
            new()
            {
                Id = "2",
                Fullname = "t3_2",
                Subreddit = "sr2"
            }
        });

        dataStore.InsertOrUpdate(new RedditPost[]
        {
            new()
            {
                Id = "2",
                Fullname = "t3_2",
                Subreddit = "sr2",
                Upvotes = 123,
            },
            new()
            {
                Id = "3",
                Fullname = "t3_3",
                Subreddit = "sr2"
            },
        });

        var sr2Posts = dataStore.QueryPosts("sr2").ToList();
        CollectionAssert.AreEquivalent(new[] { "2", "3" }, sr2Posts.Select(x => x.Id));

        Assert.That(sr2Posts.First(x => x.Id == "2").Upvotes, Is.EqualTo(123));
    }
}
