// See https://aka.ms/new-console-template for more information

using MyAsyncAwait.Task;

Console.WriteLine("Hello!");

var mainTask = MyTask.Run(MainTaskImpl);
mainTask.Wait();

return;

IEnumerable<MyTask> MainTaskImpl()
{
    Console.WriteLine("MyWritableTask test process");
    Console.WriteLine("给我随便填个文件名，不然我就一直跑圈");

    using var paoQuanTokenSource = new CancellationTokenSource();
    var paoQuanTask = PaoQuan(paoQuanTokenSource.Token);

    var filePath = Console.ReadLine() ?? "";
    var readFileTask = ReadFile(filePath);
    Console.WriteLine("让我康康文件里有啥");
    yield return readFileTask;
    var fileContent = readFileTask.Result;
    Console.WriteLine(fileContent);
    paoQuanTokenSource.Cancel();
    yield return paoQuanTask;
}

MyTask PaoQuan(CancellationToken token)
{
    return MyTask.Run(PaoQuanImpl);

    IEnumerable<MyTask> PaoQuanImpl()
    {
        var i = 0;
        while (!token.IsCancellationRequested)
        {
            ++i;
            Console.WriteLine("夜轮大大简直是天使！啊啊啊啊激动到跑圈！！！！（跑了{0}圈）", i);
            yield return MyTask.Delay(1000);
        }
    }
}

MyTask<string> ReadFile(string filePath)
{
    if (!File.Exists(filePath))
        return MyTask.FromResult("你填了个什么寄巴文件名");

    var file = File.Open(filePath, FileMode.Open, FileAccess.Read);
    var reader = new StreamReader(file);
    var result = new MyWritableTask<string>();

    reader.ReadToEndAsync()
        .ContinueWith(s => result.SetResult(s.Result));
    return result;
}
