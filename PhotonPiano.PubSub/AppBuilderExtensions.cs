using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace PhotonPiano.PubSub;

public static class AppBuilderExtensions
{
    public static void AddPubSub(this WebApplicationBuilder builder, Action<PubSubConfig> configure = null)
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services.TryAddSingleton(_ =>
        {
            var config = new PubSubConfig();
            configure?.Invoke(config);
            return config;
        });

        builder.Services.AddSignalR();
        builder.Services.AddSingleton<IPubSubService, PubSubService>();
    }

    public static void MapPubSub(this WebApplication app)
    {
        if (app is null)
            throw new ArgumentNullException(nameof(app));

        app.MapHub<PubSubHub>("/pubsub");
    }
}