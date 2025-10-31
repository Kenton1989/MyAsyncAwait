namespace MyAsyncAwait;

internal class TaskStateBox<TResult>
{
    public required IEnumerator<MyTask> PendingTasks { get; init; }
    public required MyWritableTask<TResult> ResultTask { get; init; }
}