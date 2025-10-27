using AwesomeAssertions;
using MyAsyncAwait;

namespace MyAsyncAwaitTest;

public class TaskTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TaskIncompletedTest()
    {
        var task = new MyTask();

        task.IsCompleted.Should().BeFalse();
        FluentActions.Invoking(() => task.CheckException()).Should().Throw<TaskIncompletedException>();
    }

    [Test]
    public void TaskCompletedTest()
    {
        var writableTask = new MyWritableTask();
        MyTask task = writableTask;

        writableTask.SetResult();

        task.IsCompleted.Should().BeTrue();
        FluentActions.Invoking(() => task.CheckException()).Should().NotThrow();
    }

    [Test]
    public void GenericTaskIncompletedTest()
    {
        var task = new MyTask<int>();

        task.IsCompleted.Should().BeFalse();
        FluentActions.Invoking(() => task.CheckException()).Should().Throw<TaskIncompletedException>();
        FluentActions.Invoking(() => task.Result).Should().Throw<TaskIncompletedException>();
    }

    [Test]
    public void GenericTaskCompletedTest()
    {
        var writableTask = new MyWritableTask<int>();
        MyTask<int> task = writableTask;

        writableTask.SetResult(1);

        task.IsCompleted.Should().BeTrue();
        task.Result.Should().Be(1);
        FluentActions.Invoking(() => task.CheckException()).Should().NotThrow();
    }

    [Test]
    public void TaskCallbackTest()
    {
        var writableTask = new MyWritableTask();
        MyTask task = writableTask;
        var pass = false;

        task.OnComplete(_ => { pass = true; });

        writableTask.SetResult();

        pass.Should().BeTrue();
    }

    [Test]
    public void GenericTaskCallbackTest()
    {
        var writableTask = new MyWritableTask<int>();
        MyTask<int> task = writableTask;
        var receivedResult = 0;

        task.OnComplete((value, _) => { receivedResult = value; });

        writableTask.SetResult(1);

        receivedResult.Should().Be(1);
    }

    [Test]
    public void TaskWithExceptionTest()
    {
        var writableTask = new MyWritableTask();
        MyTask task = writableTask;
        var testException = new TestException();

        writableTask.SetException(testException);

        task.IsCompleted.Should().BeTrue();
        task.Exception.Should().Be(testException);
        FluentActions.Invoking(() => task.CheckException())
            .Should().Throw<TestException>();
    }

    [Test]
    public void GenericTaskWithExceptionTest()
    {
        var writableTask = new MyWritableTask<int>();
        MyTask<int> task = writableTask;
        var testException = new TestException();

        writableTask.SetException(testException);

        task.IsCompleted.Should().BeTrue();
        task.Exception.Should().Be(testException);
        FluentActions.Invoking(() => task.Result)
            .Should().Throw<TestException>();
    }

    [Test]
    public void TaskExceptionCallbackTest()
    {
        var writableTask = new MyWritableTask();
        MyTask task = writableTask;
        var testException = new TestException();
        Exception? receivedException = null;

        task.OnComplete(exception => { receivedException = exception; });
        writableTask.SetException(testException);

        receivedException.Should().Be(testException);
    }

    [Test]
    public void GenericTaskExceptionCallbackTest()
    {
        var writableTask = new MyWritableTask<int>();
        MyTask<int> task = writableTask;
        var testException = new TestException();
        Exception? receivedException = null;

        task.OnComplete((_, exception) => { receivedException = exception; });
        writableTask.SetException(testException);

        receivedException.Should().Be(testException);
    }

    [Test]
    public void TaskCallbackAfterCompleteTest()
    {
        var writableTask = new MyWritableTask();
        MyTask task = writableTask;
        var pass = false;

        writableTask.SetResult();
        task.OnComplete(_ => { pass = true; });

        pass.Should().BeTrue();
    }

    [Test]
    public void GenericTaskCallbackAfterCompleteTest()
    {
        var writableTask = new MyWritableTask<int>();
        MyTask<int> task = writableTask;
        var receivedResult = 0;

        writableTask.SetResult(1);
        task.OnComplete((value, _) => { receivedResult = value; });

        receivedResult.Should().Be(1);
    }
}