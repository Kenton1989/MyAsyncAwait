namespace MyAsyncAwait;

internal static class TaskRunner
{
    public static MyTask Run(Func<IEnumerable<MyTask>> tasks)
    {
        var resultTask = new MyWritableTask();
        var generator = tasks();

        foreach (var task in generator)
        {
            continue;
        }

        if (!resultTask.IsCompleted) resultTask.SetResult();

        return resultTask;
    }
}