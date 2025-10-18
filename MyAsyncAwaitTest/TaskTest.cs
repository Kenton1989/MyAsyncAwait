using FluentAssertions;
using MyAsyncAwait;

namespace MyAsyncAwaitTest;

public class TaskTest
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void IncompletTaskTest()
    {
        var task = new MyTask();
        task.IsCompleted.Should().BeFalse();
    }

    [Test]
    public void CompletTaskTest()
    {
        var writableTask = new MyWritableTask();
        MyTask task = writableTask;

        task.IsCompleted.Should().BeFalse();

        writableTask.SetResult();

        task.IsCompleted.Should().BeTrue();
    }

    [Test]
    public void IncompletTaskWithResultTest()
    {
        var task = new MyTask<int>();

        task.IsCompleted.Should().BeFalse();
        task.Result.Should().Be(0);
    }

    [Test]
    public void CompletTaskWithResultTest()
    {
        var writableTask = new MyWritableTask<int>();
        MyTask<int> task = writableTask;

        writableTask.SetResult(1);

        task.IsCompleted.Should().BeTrue();
        task.Result.Should().Be(1);
    }
}
