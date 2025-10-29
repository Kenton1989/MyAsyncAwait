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
        var testPrecision = TimeSpan.FromMilliseconds(100);
        DateTimeOffset taskYieldTime = default, taskContinueTime = default;

        var startTime = DateTimeOffset.Now;
        var testTask = MyTask.Run(TestFunction);
        testTask.Wait();
        var endTime = DateTimeOffset.Now;

        taskYieldTime.Should().NotBe(default);
        taskContinueTime.Should().NotBe(default);

        testTask.IsCompleted.Should().BeTrue();
        var totalDuration = endTime - startTime;
        var yieldToContinue = taskContinueTime - taskYieldTime;
        var continueToEndTime = endTime - taskContinueTime;

        totalDuration.Should().BeCloseTo(testDelay, testPrecision);
        yieldToContinue.Should().BeCloseTo(testDelay, testPrecision);
        continueToEndTime.Should().BeCloseTo(TimeSpan.Zero, testPrecision);

        return;

        IEnumerable<MyTask> TestFunction()
        {
            var task = DelayOneSecond();
            taskYieldTime = DateTimeOffset.Now;
            yield return task;
            taskContinueTime = DateTimeOffset.Now;
            task.Wait();
        }

        MyTask DelayOneSecond()
        {
            var task = new MyWritableTask();

            _ = new Timer(
                _ => task.SetResult(),
                null,
                (int)testDelay.TotalMilliseconds,
                -1
            );

            return task;
        }
    }
}