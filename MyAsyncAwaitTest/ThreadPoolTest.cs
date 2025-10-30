using AwesomeAssertions;
using MyAsyncAwait;

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
}