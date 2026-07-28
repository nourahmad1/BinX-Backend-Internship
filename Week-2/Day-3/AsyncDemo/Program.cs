using System.Diagnostics;
using  System.Threading;
Stopwatch stopwatch= Stopwatch.StartNew();
    await GetDatabaseDataAsync();
    await GetApiDataAsync();
    await GetFileDataAsync();
    stopwatch.Stop();
    Console.WriteLine($"Sequential Time: {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine();
    Console.WriteLine("Concurrent Execution");
    stopwatch.Restart();
    Task databaseTask = GetDatabaseDataAsync();
    Task apiTask = GetApiDataAsync();
    Task fileTask = GetFileDataAsync();
    await Task.WhenAll(databaseTask, apiTask, fileTask);
    stopwatch.Stop();
    Console.WriteLine($"Concurrent Time: {stopwatch.ElapsedMilliseconds} ms");
    Console.WriteLine();
    Console.WriteLine("Cancellation Demo");
    CancellationTokenSource cts =new();
    Task cancellationTask = GetDatabaseWithCancelationDataAsync(cts.Token);
    await Task.Delay(2000);
    cts.Cancel();
    try
    {
      await cancellationTask;
    }
    catch(OperationCanceledException)
   {
    Console.WriteLine("Database Operation was cancelled");
   }



static async Task GetDatabaseDataAsync()
{
    Console.WriteLine("Reading Database ...");
    await Task.Delay(2000);
    Console.WriteLine("Database Done");

}

static async Task GetApiDataAsync()
{
    Console.WriteLine("Calling API ...");
    await Task.Delay(3000);
    Console.WriteLine("API Done");
    
}
static async Task GetFileDataAsync()
{
    Console.WriteLine("Reading File ...");
    await Task.Delay(1000);
    Console.WriteLine("Reading File Done");
    
}

static async Task GetDatabaseWithCancelationDataAsync(CancellationToken token)
{
    Console.WriteLine("Reading Database with cancelation ...");
    await Task.Delay(5000, token);
    Console.WriteLine("Database Done");

}

//static async Task Main (string[] args)
//{   
//}


