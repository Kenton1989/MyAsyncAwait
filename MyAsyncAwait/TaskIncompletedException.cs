namespace MyAsyncAwait;

public class TaskIncompletedException(string theTaskHasNotCompleted) : Exception(theTaskHasNotCompleted);