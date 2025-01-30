using System.Linq.Expressions;

namespace PhotonPiano.BackgroundJob;

public interface IDefaultScheduleJob
{
    string Enqueue<T>(Expression<Action<T>> methodCall);
}