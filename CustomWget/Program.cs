using System.ComponentModel.DataAnnotations;

namespace CustomWget;

internal static class Program
{
    private static readonly CancellationTokenSource cts = new();
    
    private static async Task Main(string[] args)
    {
        var (url, fileName) = ParseArgs(args);

        Console.CancelKeyPress += OnCtrlC!;
        
        var downloader = new Downloader();
        
        await downloader.DownloadWithInfo(url, fileName, cts.Token);
    }
    
    private static void OnCtrlC(object sender, ConsoleCancelEventArgs args)
    {
        args.Cancel = true;
        cts.Cancel();
        
        Console.WriteLine("Cleaning up. please wait...");
    }

    private static (string url, string fileName) ParseArgs(IReadOnlyList<string> args)
    {
        if (args.Count != 1)
        {
            throw new ValidationException("Please use like 'customWget google.com/file.pdf'");
        }
            
        var url = args[0];
        var fileName = url.Split('/').LastOrDefault();
        if (string.IsNullOrEmpty(fileName) || !fileName.Contains('.'))
        {
            throw new ValidationException("Please specify file name with extension");
        }
            
        return (url, fileName);
    }
}