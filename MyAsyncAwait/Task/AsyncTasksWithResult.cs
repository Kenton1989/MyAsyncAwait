namespace MyAsyncAwait.Task;

public delegate IEnumerable<MyTask> AsyncTasksWithResult<TResult>(MyWritableTask<TResult> tasks);