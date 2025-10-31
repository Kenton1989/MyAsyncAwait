using AwesomeAssertions;
using MyAsyncAwait;
using MyAsyncAwait.ThreadPool;
using NUnit.Framework.Internal;

namespace MyAsyncAwaitTest;

public class ThreadPoolTest
{
    private MyThreadPool _target;

    [SetUp]
    public void Setup()
    {
        _target = MyThreadPool.Instance;
    }

    [Test]
    public void RunOneTask()
    {
        var passSignal = new ManualResetEvent(false);
        
        _target.QueueWork(TestFunc);
        
        var pass = passSignal.WaitOne(TimeSpan.FromSeconds(1));

        pass.Should().BeTrue();

        return;

        void TestFunc()
        {
            passSignal.Set();
        }
    }

    [Test]
    public void RunTwoTask()
    {
        var testDelay = TimeSpan.FromSeconds(1);
        var testPrecision = TimeSpan.FromMilliseconds(100);

        var startTime = DateTimeOffset.Now;

        var signal1 = new ManualResetEvent(false);
        _target.QueueWork(() => SleepAndSet(signal1));

        var signal2 = new ManualResetEvent(false);
        _target.QueueWork(() => SleepAndSet(signal2));

        var queueTime = DateTimeOffset.Now;
        
        var func1Completed = signal1.WaitOne(TimeSpan.FromSeconds(3));
        var func2Completed = signal2.WaitOne(TimeSpan.FromSeconds(3));

        var completeTime = DateTimeOffset.Now;
        
        var queueDuration = queueTime - startTime;
        var executeDuration = completeTime - startTime;
        
        func1Completed.Should().BeTrue();
        func2Completed.Should().BeTrue();
        queueDuration.Should().BeCloseTo(TimeSpan.Zero, testPrecision);
        executeDuration.Should().BeCloseTo(testDelay, testPrecision);

        return;

        void SleepAndSet(ManualResetEvent resetEvent)
        {
            Thread.Sleep(testDelay);
            resetEvent.Set();
        }
    }

    [Test]
    public void ThreadIsNotInThePool()
    {
        _target.IsMyThread(Environment.CurrentManagedThreadId).Should().BeFalse();
    }


    [Test]
    public void ThreadPoolIsInThePool()
    {
        var threadId = 0;
        var doneSignal = new ManualResetEvent(false);
        
        _target.QueueWork(TestFunc);
        var done = doneSignal.WaitOne(TimeSpan.FromSeconds(1));
        
        done.Should().BeTrue();
        threadId.Should().NotBe(0);
        _target.IsMyThread(threadId).Should().BeTrue();

        return;

        void TestFunc()
        {
            threadId = Environment.CurrentManagedThreadId;
            doneSignal.Set();
        }
    }
}