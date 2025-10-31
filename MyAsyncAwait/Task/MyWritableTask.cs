namespace MyAsyncAwait.Task;

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