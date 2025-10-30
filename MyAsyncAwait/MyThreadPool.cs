namespace MyAsyncAwait;

public class MyThreadPool
{
    public static MyThreadPool Instance { get; } = new MyThreadPool();

    private MyThreadPool(int threadCount = 4)
    {
    }

    public void QueueWork(Action func)
    {
        func();
    }
}