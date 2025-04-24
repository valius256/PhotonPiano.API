using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PhotonPiano.PubSub.Notification;
using PhotonPiano.PubSub.Pubsub;
using PhotonPiano.PubSub.StudentClassScore;

namespace PhotonPiano.PubSub;

public static class AppBuilderExtensions
{
    public static void AddSignalRConfig(this WebApplicationBuilder builder)
    {
        if (builder is null)
            throw new ArgumentNullException(nameof(builder));

        builder.Services.AddSignalR(cfg => cfg.EnableDetailedErrors = true);
        builder.Services.AddSingleton<IPubSubService, PubSubService>();
        builder.Services.AddSingleton<INotificationServiceHub, NotificationServiceHub>();
        builder.Services.AddSingleton<IStudentClassScoreServiceHub, StudentClassScoreServiceHub>();
    }

    public static void MapSignalRConfig(this WebApplication app)
    {
        if (app is null)
            throw new ArgumentNullException(nameof(app));

        app.MapHub<PubSubHub>("/pubsub");
        app.MapHub<NotificationHub>("/notification");
        app.MapHub<ProgressHub>("/progress");
        app.MapHub<StudentClassScoreHub>("/score");
    }
}