namespace MyAsyncAwait;

internal class TaskRunner
{
    public static readonly TaskRunner Instance = new();

    private TaskRunner()
    {
    }

    public MyTask Run(Func<IEnumerable<MyTask>> tasks)
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

    private void QueueProcessTask(TaskStateBox box)
    {
        MyThreadPool.Instance.QueueWork(() => ProcessTask(box));
    }

    private void ProcessTask(TaskStateBox box)
    {
        var pendingTasks = box.PendingTasks;
        var resultTask = box.ResultTask;

        try
        {
            if (pendingTasks.MoveNext())
            {
                var pendingTask = pendingTasks.Current;
                pendingTask.ContinueWith(e => { QueueProcessTask(box); });
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