namespace MyAsyncAwait;

public delegate IEnumerable<MyTask> AsyncTasksWithResult<TResult>(MyWritableTask<TResult> tasks);