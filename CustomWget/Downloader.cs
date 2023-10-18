using System.Diagnostics;

namespace CustomWget;

#pragma warning disable CS4014

public static class Downloader
{
    public static async Task DownloadWithInfoAsync(string url, string fileName, CancellationToken ct = default)
    {
        var progress = new Progress<string>(ProgressHandler);

        await DownloadAsync(url, fileName, ct, progress);

        if (ct.IsCancellationRequested)
        {
            CleanUp(fileName);
        }
    }

    private static void ProgressHandler(string x)
    {
        var whiteSpace = new string(' ', Console.WindowWidth - x.Length);
        Console.Write("\r{0}{1}", x, whiteSpace);
    }

    private static async Task DownloadAsync(string url, string fileName, CancellationToken ct,
        IProgress<string> progress)
    {
        CleanUp(fileName);

        var httpClient = new HttpClient();
        using var response = await httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
        await using var downloadStream = await response.Content.ReadAsStreamAsync(ct);
        await using var fileStream = new FileStream(fileName, FileMode.OpenOrCreate);
        var copyTask = downloadStream.CopyToAsync(fileStream, ct);

        var fileSize = response.Content.Headers.ContentLength ?? 0;

        await ProgressUpdatingAsync(copyTask, fileStream, fileSize, ct, progress);
    }

    private static async Task ProgressUpdatingAsync(Task copyTask, Stream fileStream, long fileSize,
        CancellationToken ct, IProgress<string> progress)
    {
        var onePercent = fileSize / 100;

        while (!copyTask.IsCompleted)
        {
            var downloaded = fileStream.Position;
            progress.Report(
                $"{ByteFormatter.Format(downloaded)}/{ByteFormatter.Format(fileSize)} ({downloaded / onePercent}%) | AllocatedMemory: {GetAllocatedMemory()}");
            await Task.Delay(300, ct).IgnoreOnCancellationAsync();
        }
        
        var canceledCause = copyTask.IsCanceled ? "Canceled" : "Faulted";
        progress.Report(copyTask.IsCompletedSuccessfully ? "Done!" : canceledCause);
    }

    private static void CleanUp(string fileName)
    {
        if (File.Exists(fileName))
        {
            File.Delete(fileName);
        }
    }

    private static string GetAllocatedMemory()
    {
        var proc = Process.GetCurrentProcess();
        return ByteFormatter.Format(proc.PrivateMemorySize64);
    }
}