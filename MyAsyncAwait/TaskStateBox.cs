namespace MyAsyncAwait;

internal class TaskStateBox
{
    public required IEnumerator<MyTask> PendingTasks { get; init; }
    public required MyWritableTask ResultTask { get; init; }
}