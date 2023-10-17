using System.ComponentModel.DataAnnotations;

namespace CustomWget;

internal static class Program
{
    private static readonly CancellationTokenSource Cts = new();

    private static async Task Main(string[] args)
    {
        Console.CancelKeyPress += OnCtrlC!;

        await Execute(args).SuppressWithMessage();
    }
    
    private static async Task Execute(IReadOnlyList<string> args)
    {
        var (url, fileName) = ParseArgs(args);
        await Downloader.DownloadWithInfoAsync(url, fileName, Cts.Token);
    }

    private static void OnCtrlC(object sender, ConsoleCancelEventArgs args)
    {
        args.Cancel = true;
        Cts.Cancel();

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