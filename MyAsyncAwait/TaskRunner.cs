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
        var resultTask = new MyWritableTask<MyVoidType>();
        var box = new TaskStateBox<MyVoidType>
        {
            PendingTasks = generator.GetEnumerator(),
            ResultTask = resultTask
        };

        ProcessTask(box);

        return box.ResultTask;
    }

    public MyTask<TResult> Run<TResult>(AsyncTasksWithResult<TResult> tasksWithResult)
    {
        var result = new MyWritableTask<TResult>();
        
        var generator = tasksWithResult(result);
        var box = new TaskStateBox<TResult>
        {
            PendingTasks = generator.GetEnumerator(),
            ResultTask = result
        };
        
        ProcessTask(box);

        return result;
    }

    private void QueueProcessTask<TResult>(TaskStateBox<TResult> box)
    {
        MyThreadPool.Instance.QueueWork(() => ProcessTask(box));
    }

    private void ProcessTask<TResult>(TaskStateBox<TResult> box)
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
                pendingTasks.Dispose();

                var voidResultTask = resultTask as MyWritableTask<MyVoidType>;
                voidResultTask?.SetResult(MyVoidType.Void);

                if (!resultTask.IsCompleted)
                {
                    throw new TaskNotResultedException();
                }
            }
        }
        catch (Exception ex)
        {
            resultTask.SetException(ex);
        }
    }
}