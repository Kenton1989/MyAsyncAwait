using FluentAssertions;
using MyAsyncAwait;

namespace MyAsyncAwaitTest;

public class TaskRunnerTest
{
    [Test]
    public void TestSimpleTask()
    {
        var pass = false;

        var testTask = MyTask.Run(SimpleTask);

        testTask.IsCompleted.Should().BeTrue();
        pass.Should().BeTrue();

        return;
        IEnumerable<MyTask> SimpleTask(MyWritableTask resultTask)
        {
            pass = true;
            yield break;
        }
    }

    [Test]
    public void TestImmediatelyCompletedCallTask()
    {
        var pass = false;

        var testTask = MyTask.Run(ImmediatelyCompleted);

        testTask.IsCompleted.Should().BeTrue();
        pass.Should().BeTrue();

        return;

        MyTask ImmediateCompleted()
        {
            return MyTask.CompletedTask;
        }

        IEnumerable<MyTask> ImmediatelyCompleted(MyWritableTask resultTask)
        {
            var task = ImmediateCompleted();
            yield return task;
            task.CheckException();

            pass = true;
        }
    }
}