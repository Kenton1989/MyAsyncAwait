// See https://aka.ms/new-console-template for more information

using MyAsyncAwait.Task;

Console.WriteLine("Hello, World!");

return;

MyTask PaoQuan(CancellationToken token)
{
    return new MyTask();
}

MyTask<string> ReadFile(string filePath)
{
    return new MyTask<string>();
}

IEnumerable<MyTask> MainTaskImpl(MyWritableTask resultTask)
{
    Console.WriteLine("MyWritableTask test process");

    using var paoQuanTokenSource = new CancellationTokenSource();
    var paoQuanTask = PaoQuan(paoQuanTokenSource.Token);

    Console.WriteLine("Input a filename, otherwise I will continue Pao Quan.");
    var filePath = Console.ReadLine() ?? "";
    var readFileTask = ReadFile(filePath);
    yield return readFileTask;
    var fileContent = readFileTask.Result;
    Console.WriteLine("I see the file has:\n" + fileContent);

    paoQuanTokenSource.Cancel();
    yield return paoQuanTask;
    paoQuanTask.Wait();

    resultTask.SetResult();
}