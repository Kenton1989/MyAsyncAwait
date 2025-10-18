# MyAsyncAwait

implement something similar to async/await in C# without using the keyword async & await.

## Requirement
- ```async``` & ```await``` keywords are not allowed to be used
- allow using ```yield``` to create generator
- Similar to async/await, no callbacks will used when writing the control flow.

## Test Case
able to write something similar to this
``` C#
{
    var tokenSource = new();
    // pao quan every 1 second in the background
    var paoQuanTask = paoQuan(tokenSource.Token);

    var filename = input();
    // read first 1000 bytes of file
    var readFileTask = readFile(filename, 1000);
    yield return readFileTask;
    var fileContent = readFileTask.Result;
    print(fileContent)

    tokenSource.Cancel();
    yield return paoQuanTask;
    paoQuanTask.Wait();

    print("done")
}
```

## Reference

[How Async/Await Really Works in C#](https://devblogs.microsoft.com/dotnet/how-async-await-really-works/)