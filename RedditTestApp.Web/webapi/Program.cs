using RedditTestApp.RedditClient;
using RedditTestApp.RedditClient.Models;
using RedditTestApp.RedditClient.Options;
using RedditTestApp.Worker;

namespace RedditTestApp.Api;

public static class Program
{
    public static void Main(params string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        ConfigureServices(builder, builder.Configuration, builder.Services);

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }

    private static void ConfigureServices(WebApplicationBuilder builder, ConfigurationManager config, IServiceCollection services)
    {
        services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();

        services.Configure<RedditOptions>(config.GetSection("Reddit"))
                .AddRedditClient();

        var subreddits = config.GetSection("Subreddits").Get<List<string>>();
        if (subreddits != null)
        {
            services.AddSubredditMonitor(subreddits);
            services.AddHostedService<UpdateRedditPostsHostedService>();
        }

        services.AddSingleton<RedditPostDataStore>()
                .AddSingleton<IWorkerDataStore<RedditPost>>(p => p.GetRequiredService<RedditPostDataStore>())
                .AddSingleton<IRedditPostDataStore>(p => p.GetRequiredService<RedditPostDataStore>());
    }
}