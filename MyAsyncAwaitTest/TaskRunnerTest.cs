using AwesomeAssertions;
using MyAsyncAwait.Task;
using MyAsyncAwait.ThreadPool;

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
        testTask.Wait();

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

    [Test]
    public void MyTaskShouldUseMyThreadPool()
    {
        int threadId1 = 0, threadId2 = 0;
        var testTask = MyTask.Run(TestFunction);
        testTask.Wait();

        testTask.IsCompleted.Should().BeTrue();
        threadId1.Should().Be(Environment.CurrentManagedThreadId);
        MyThreadPool.Instance.IsMyThread(threadId2).Should().BeTrue();

        return;

        IEnumerable<MyTask> TestFunction()
        {
            threadId1 = Environment.CurrentManagedThreadId;
            var task = MyTask.CompletedTask;
            yield return task;
            task.Wait();
            threadId2 = Environment.CurrentManagedThreadId;
        }
    }

    [Test]
    public void TaskWithExceptionShouldThrow()
    {
        var testTask = MyTask.Run(TestFunction);

        FluentActions.Invoking(() => testTask.Wait())
            .Should().Throw<TestException>();

        return;

        IEnumerable<MyTask> TestFunction()
        {
            var task = TriggerException();
            yield return task;
            task.Wait();
        }

        MyTask TriggerException()
        {
            var myTask = new MyWritableTask();
            myTask.SetException(new TestException());
            return myTask;
        }
    }

    [Test]
    public void GenericTestSimpleTask()
    {
        var testTask = MyTask.Run<int>(SimpleTask);

        testTask.IsCompleted.Should().BeTrue();
        testTask.Result.Should().Be(0);

        return;

        IEnumerable<MyTask> SimpleTask(MyWritableTask<int> resultTask)
        {
            resultTask.SetResult(0);
            yield break;
        }
    }

    [Test]
    public void GenericTestImmediatelyCompletedTask()
    {
        var testTask = MyTask.Run<int>(TestFunction);

        testTask.Result.Should().Be(42);
        testTask.IsCompleted.Should().BeTrue();

        return;

        MyTask<int> ImmediateCompleted()
        {
            return MyTask.FromResult(42);
        }

        IEnumerable<MyTask> TestFunction(MyWritableTask<int> resultTask)
        {
            var task = ImmediateCompleted();
            yield return task;
            resultTask.SetResult(task.Result);
        }
    }

    [Test]
    public void GenericTestDelayedCompletedTask()
    {
        var testDelay = TimeSpan.FromSeconds(1);
        var testPrecision = TimeSpan.FromMilliseconds(100);
        DateTimeOffset taskYieldTime = default, taskContinueTime = default;

        var startTime = DateTimeOffset.Now;
        var testTask = MyTask.Run<int>(TestFunction);
        var result = testTask.Result;
        var endTime = DateTimeOffset.Now;

        taskYieldTime.Should().NotBe(default);
        taskContinueTime.Should().NotBe(default);

        testTask.IsCompleted.Should().BeTrue();
        result.Should().Be(42);
        var totalDuration = endTime - startTime;
        var yieldToContinue = taskContinueTime - taskYieldTime;
        var continueToEndTime = endTime - taskContinueTime;

        totalDuration.Should().BeCloseTo(testDelay, testPrecision);
        yieldToContinue.Should().BeCloseTo(testDelay, testPrecision);
        continueToEndTime.Should().BeCloseTo(TimeSpan.Zero, testPrecision);

        return;

        IEnumerable<MyTask> TestFunction(MyWritableTask<int> resultTask)
        {
            var task = DelayOneSecond();
            taskYieldTime = DateTimeOffset.Now;
            yield return task;
            taskContinueTime = DateTimeOffset.Now;
            resultTask.SetResult(task.Result);
        }

        MyTask<int> DelayOneSecond()
        {
            var task = new MyWritableTask<int>();

            _ = new Timer(
                _ => task.SetResult(42),
                null,
                (int)testDelay.TotalMilliseconds,
                -1
            );

            return task;
        }
    }
}