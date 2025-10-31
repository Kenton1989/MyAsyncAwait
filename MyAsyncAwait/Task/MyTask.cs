using MyAsyncAwait.Runner;

namespace MyAsyncAwait.Task;

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

    public static MyTask<TResult> Run<TResult>(AsyncTasksWithResult<TResult> tasksWithResult)
    {
        return TaskRunner.Instance.Run(tasksWithResult);;
    }
    
    public static MyTask Delay(int milliseconds)
    {
        var task = new MyWritableTask();
        _ = new Timer(
            _ => task.SetResult(),
            null,
            milliseconds,
            -1
        );
        return task;
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
