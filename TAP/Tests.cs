using System.Diagnostics;
namespace Fun;

public class Tests
{
    [Test]
    public async Task WoopyDoopy_WithVar()
    {
        var timeSpan = TimeSpan.FromSeconds(2);

        var stopwatch = Stopwatch.StartNew();

        var func = () => SyncDelay(timeSpan);
        await (func, func);
        
        stopwatch.Stop();
        
        // only 2 seconds elapsed instead of 4
        Assert.That((int)stopwatch.Elapsed.TotalSeconds, Is.EqualTo((int)timeSpan.TotalSeconds));
    }
    
    [Test]
    public async Task WoopyDoopy_WithAnotherBrackets()
    {
        var timeSpan = TimeSpan.FromSeconds(2);

        var stopwatch = Stopwatch.StartNew();

        await new [] {() => SyncDelay(timeSpan), () => SyncDelay(timeSpan)}; // runs in parallel
        
        stopwatch.Stop();
        
        // only 2 seconds elapsed instead of 4
        Assert.That((int)stopwatch.Elapsed.TotalSeconds, Is.EqualTo((int)timeSpan.TotalSeconds));
    }

    private Task SyncDelay(TimeSpan timeSpan)
    {
        Thread.Sleep(timeSpan);
        return Task.CompletedTask;
    }
}