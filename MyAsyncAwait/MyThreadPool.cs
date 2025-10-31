using System.Collections.Concurrent;

namespace MyAsyncAwait;

public class MyThreadPool : IDisposable
{
    public static MyThreadPool Instance { get; } = new();

    private readonly Thread[] _workerThreads;
    private readonly ConcurrentQueue<Action> _workItems = new();
    private readonly AutoResetEvent _workItemQueued = new(false);

    private MyThreadPool(int threadCount = 8)
    {
        _workerThreads = Enumerable.Range(0, threadCount)
            .Select(CreateThread)
            .ToArray();
    }

    private Thread CreateThread(int idx)
    {
        var thread = new Thread(Work)
        {
            IsBackground = true,
            Name = $"{nameof(MyThreadPool)}.Worker.{idx}",
            Priority = ThreadPriority.Normal
        };

        thread.Start();
        return thread;
    }

    public void QueueWork(Action func)
    {
        _workItems.Enqueue(func);
        _workItemQueued.Set();
    }

    private void Work(object? obj)
    {
        while (!IsDisposed)
        {
            try
            {
                if (_workItems.TryDequeue(out var workItem))
                {
                    workItem();
                }
                else
                {
                    _workItemQueued.WaitOne(100);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("{0} Exception: {1}", Thread.CurrentThread.Name, ex);
            }
        }
    }

    private int _disposedFlag;
    private bool IsDisposed => _disposedFlag == 1;
    private bool TrySetDisposed()
    {
        var oldValue = Interlocked.CompareExchange(ref _disposedFlag, 1, 0);
        return oldValue == 0;
    }

    public void Dispose()
    {
        if (TrySetDisposed())
        {
            foreach (var workerThread in _workerThreads)
            {
                workerThread.Join();
            }

            GC.SuppressFinalize(this);
        }
        else
        {
            throw new ObjectDisposedException(nameof(MyThreadPool));
        }
    }
}