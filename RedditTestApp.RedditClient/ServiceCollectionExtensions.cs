using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.RateLimiting;

namespace RedditTestApp.RedditClient;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRedditClient(this IServiceCollection services)
    {
        services.AddSingleton<IRedditAuthService, RedditAuthService>()
                .AddSingleton<IRedditClient, RedditClient>(p =>
                {
                    var limiter = new SlidingWindowRateLimiter(
                        new SlidingWindowRateLimiterOptions
                        {
                            Window = TimeSpan.FromMinutes(1),
                            PermitLimit = 75,
                            SegmentsPerWindow = 60,
                        });

                    return new RedditClient(p.GetRequiredService<IRedditAuthService>(), limiter);
                });

        return services;
    }
}