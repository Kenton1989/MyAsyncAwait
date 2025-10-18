namespace MyAsyncAwait;

public class MyTask
{
    public bool IsCompleted { get; protected set; }
}

public class MyTask<TResult>: MyTask
{
    public TResult? Result { get; protected set; }
}

public class MyWritableTask : MyTask
{
    public void SetResult()
    {
        IsCompleted = true;
    }
}

public class MyWritableTask<TResult> : MyTask<TResult>
{
    public void SetResult(TResult result)
    {
        IsCompleted = true;
        Result  = result;
    }
}

