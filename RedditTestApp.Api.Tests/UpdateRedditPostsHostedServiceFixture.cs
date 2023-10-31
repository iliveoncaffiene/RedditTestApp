using Microsoft.Extensions.DependencyInjection;
using RedditTestApp.RedditClient;
using RedditTestApp.RedditClient.Models;

namespace RedditTestApp.Api.Tests;

[TestFixture]
public class UpdateRedditPostsHostedServiceFixture
{
    private UpdateRedditPostsHostedService service;
    private Mock<IServiceProvider> mockServiceProvider;
    private Mock<IServiceScopeFactory> mockScopeFactory;
    private Mock<IServiceScope> mockScope;

    [SetUp]
    public void Setup()
    {
        mockServiceProvider = new Mock<IServiceProvider>();
        mockScopeFactory = new Mock<IServiceScopeFactory>();
        mockScope = new Mock<IServiceScope>();
        mockScope.SetupGet(x => x.ServiceProvider)
            .Returns(mockServiceProvider.Object)
            .Verifiable(); // just re-use the service provider, it's just a test
        mockScopeFactory.Setup(x => x.CreateScope())
            .Returns(mockScope.Object)
            .Verifiable();
        mockServiceProvider.Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(mockScopeFactory.Object)
            .Verifiable();
        service = new(mockServiceProvider.Object);
    }

    [Test]
    public async Task UpdatePosts()
    {
        var mockClient = new Mock<IRedditClient>();
        mockClient.Setup(x => x.GetUpdatedPostsByPostIds(It.IsAny<IEnumerable<string>>()))
            .Returns<IEnumerable<string>>(ids => Task.FromResult(ids.Select(id => new RedditPost
            {
                Id = id.Split('_')[1],
                Fullname = id,
                Author = "abc123",
                Subreddit = "sr1",
                Upvotes = 555,
            })))
            .Verifiable();

        mockServiceProvider.Setup(x => x.GetService(typeof(IRedditClient)))
            .Returns(mockClient.Object);

        IRedditPostDataStore dataStore = new RedditPostDataStore();
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
                Subreddit = "sr1"
            }
        });
        mockServiceProvider.Setup(x => x.GetService(typeof(IRedditPostDataStore)))
            .Returns(dataStore);

        CollectionAssert.AreEquivalent(new[] { 0, 0 }, dataStore.QueryPosts().Select(p => p.Upvotes));

        await service.UpdatePosts();

        mockClient.Verify();
        CollectionAssert.AreEquivalent(new[] { 555, 555 }, dataStore.QueryPosts().Select(p => p.Upvotes));
    }
}