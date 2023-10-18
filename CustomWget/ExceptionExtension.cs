namespace CustomWget;

public static class ExceptionExtension
{
    public static async Task IgnoreOnCancellationAsync(this Task task)
    {
        try
        {
            await task;
        }
        catch (TaskCanceledException)
        {
            //Ignore
        }
    }

    public static async Task SuppressWithMessageAsync(this Task task)
    {
        try
        {
            await task;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}