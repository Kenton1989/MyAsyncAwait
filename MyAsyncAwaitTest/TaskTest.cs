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
}
