namespace MyAsyncAwait;

public class MyTask
{
    public bool IsCompleted { get; protected set; }
    public Exception? Exception { get; protected set; }

    protected readonly List<Action> OnCompleteActions = [];

    protected void Complete()
    {
        IsCompleted = true;
        foreach (var action in OnCompleteActions)
        {
            action();
        }
    }

    public void OnComplete(Action<Exception?> action)
    {
        if (IsCompleted)
        {
            action(Exception);
            return;
        }
        OnCompleteActions.Add(() => action(Exception));
    }

    public void CheckException()
    {
        if (!IsCompleted)
        {
            throw new TaskIncompletedException("The task has not completed.");
        }

        if (Exception != null)
        {
            throw Exception;
        }
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
    private TResult? _result;
    public TResult Result
    {
        get => GetResult();
        protected set => _result = value;
    }

    private TResult GetResult()
    {
        CheckException();
        
        return _result!;
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