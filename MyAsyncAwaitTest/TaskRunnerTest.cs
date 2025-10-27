using AwesomeAssertions;
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
        IEnumerable<MyTask> SimpleTask()
        {
            pass = true;
            yield break;
        }
    }

    [Test]
    public void TestImmediatelyCompletedTask()
    {
        var pass = false;

        var testTask = MyTask.Run(TestFunction);

        testTask.IsCompleted.Should().BeTrue();
        pass.Should().BeTrue();

        return;

        MyTask ImmediateCompleted()
        {
            return MyTask.CompletedTask;
        }

        IEnumerable<MyTask> TestFunction()
        {
            var task = ImmediateCompleted();
            yield return task;
            task.Wait();

            pass = true;
        }
    }

    [Test]
    public void TestDelayedCompletedTask()
    {
        var testDelay = TimeSpan.FromSeconds(1);

        var startTime = DateTimeOffset.Now;
        var testTask = MyTask.Run(TestFunction);
        testTask.Wait();
        var endTime = DateTimeOffset.Now;

        testTask.IsCompleted.Should().BeTrue();
        (endTime - startTime).Should().BeGreaterThan(testDelay); 

        return;

        IEnumerable<MyTask> TestFunction()
        {
            var task = DelayOneSecond();
            yield return task;
            task.Wait();
        }

        MyTask DelayOneSecond()
        {
            var task = new MyWritableTask();
            Task.Delay(testDelay)
                .ContinueWith(_ => task.SetResult());
            return task;
        }
    }
}