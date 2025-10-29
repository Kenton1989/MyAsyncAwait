namespace MyAsyncAwait;

internal static class TaskRunner
{
    public static MyTask Run(Func<IEnumerable<MyTask>> tasks)
    {
        var generator = tasks();
        var box = new TaskStateBox
        {
            PendingTasks = generator.GetEnumerator(),
            ResultTask = new MyWritableTask()
        };

        ProcessTask(box);

        return box.ResultTask;
    }

    private static void ProcessTask(TaskStateBox box)
    {
        var pendingTasks = box.PendingTasks;
        var resultTask = box.ResultTask;

        try
        {
            if (pendingTasks.MoveNext())
            {
                var pendingTask = pendingTasks.Current;
                pendingTask.ContinueWith(e =>
                {
                    ProcessTask(box);
                });
            }
            else
            {
                resultTask.SetResult();
            }
        }
        catch (Exception ex)
        {
            resultTask.SetException(ex);
        }
    }
}