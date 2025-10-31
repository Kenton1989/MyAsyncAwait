namespace MyAsyncAwait;

public class MyTask
{
    private readonly ManualResetEvent _taskCompletedEvent = new(false);
    public bool IsCompleted => _taskCompletedEvent.WaitOne(0);
    public Exception? Exception { get; protected set; }

    protected readonly List<Action> OnCompleteActions = [];

    protected void Complete()
    {
        if (IsCompleted) return;

        var completedByThisThread = _taskCompletedEvent.Set();
        if (!completedByThisThread)
            return;

        foreach (var action in OnCompleteActions)
        {
            action();
        }
    }

    public void ContinueWith(Action<Exception?> action)
    {
        if (IsCompleted)
        {
            action(Exception);
            return;
        }

        OnCompleteActions.Add(() => action(Exception));
    }

    public bool Wait()
    {
        if (!_taskCompletedEvent.WaitOne())
            return false;

        return Exception != null ? throw Exception : true;
    }

    public static readonly MyTask CompletedTask = CreateCompletedTask();

    private static MyWritableTask CreateCompletedTask()
    {
        var res = new MyWritableTask();
        res.SetResult();
        return res;
    }

    public static MyTask<T> FromResult<T>(T result)
    {
        var task = new MyWritableTask<T>();
        task.SetResult(result);
        return task;
    }

    public static MyTask Run(Func<IEnumerable<MyTask>> tasks)
    {
        return TaskRunner.Instance.Run(tasks);
    }
    
    public delegate IEnumerable<MyTask> AsyncTaskWithOutput<TResult>(MyWritableTask<TResult> tasks);

    public static MyTask<TResult> Run<TResult>(AsyncTaskWithOutput<TResult> asyncTask)
    {
        var result = new MyWritableTask<TResult>();
        foreach (var _ in asyncTask(result))
        {
            
        }
        return result;
    }
}

public class MyWritableTask : MyTask
{
    public void SetResult()
    {
        Complete();
    }

    public void SetException(Exception exception)
    {
        Exception = exception;
        Complete();
    }
}

public class MyTask<TResult> : MyTask
{
    private TResult _result = default!;

    public TResult Result
    {
        get => GetResult();
        protected set => _result = value;
    }

    private TResult GetResult()
    {
        Wait();

        return _result;
    }

    public void OnComplete(Action<TResult?, Exception?> action)
    {
        if (IsCompleted)
        {
            action(_result, Exception);
            return;
        }

        OnCompleteActions.Add(() => action(_result, Exception));
    }
}

public class MyWritableTask<TResult> : MyTask<TResult>
{
    public void SetResult(TResult result)
    {
        Result = result;
        Complete();
    }

    public void SetException(Exception exception)
    {
        Exception = exception;
        Complete();
    }
}