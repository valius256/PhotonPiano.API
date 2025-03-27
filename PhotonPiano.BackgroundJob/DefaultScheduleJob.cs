using Hangfire;
using System.Linq.Expressions;

namespace PhotonPiano.BackgroundJob;

public class DefaultScheduleJob : IDefaultScheduleJob
{
    private readonly IBackgroundJobClient _backgroundJobClient;
    private readonly IServiceProvider _serviceProvider;

    public DefaultScheduleJob(
        IBackgroundJobClient backgroundJobClient,
        IServiceProvider serviceProvider
    )
    {
        _backgroundJobClient = backgroundJobClient;
        _serviceProvider = serviceProvider;
    }

    public string Enqueue<T>(Expression<Action<T>> methodCall)
    {
        var instance = _serviceProvider.GetService(typeof(T));
        if (instance == null) throw new Exception("DEFAULT_SCHEDULE_JOB_RESOLVE_INSTANCE");

        return _backgroundJobClient.Enqueue(methodCall);
    }
}