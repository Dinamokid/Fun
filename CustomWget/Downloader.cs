using System.Diagnostics;

namespace CustomWget;

#pragma warning disable CS4014

public class Downloader
{
    private readonly HttpClient _httpClient = new();

    public async Task DownloadWithInfo(string url, string fileName, CancellationToken ct = default)
    {
        var progress = new Progress<string>(x =>
        {
            Console.Clear();
            Console.WriteLine(x);
        });
        
        await Download(url, fileName, ct, progress);

        if (ct.IsCancellationRequested)
        {
            CleanUp(fileName);
        }
        
        Console.Clear();
        Console.WriteLine("Done!");
    }

    private async Task Download(string url, string fileName, CancellationToken ct, IProgress<string> progress)
    {
        CleanUp(fileName);

        using var response = await _httpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
        await using var s = await response.Content.ReadAsStreamAsync(ct);
        await using var fs = new FileStream(fileName, FileMode.OpenOrCreate);
        var copyTask = s.CopyToAsync(fs, ct);
                
        var fileSize = response.Content.Headers.ContentLength ?? 0;
        var onePercent = fileSize / 100;
                
        while (!copyTask.IsCompleted)
        {
            var downloaded = fs.Position;
            progress.Report($"{ByteFormatter.Format(downloaded)}/{ByteFormatter.Format(fileSize)} ({downloaded / onePercent}%) | AllocatedMemory: {GetAllocatedMemory()}");
            await Task.Delay(300, ct);
        }
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