namespace RedditTestApp.Worker;

public interface IWorkerDataStore<T>
{
    void InsertOrUpdate(IEnumerable<T> data);
}
