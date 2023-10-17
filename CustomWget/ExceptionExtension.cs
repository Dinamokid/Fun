namespace CustomWget;

public static class ExceptionExtension
{
    public static async Task IgnoreOnCancellation(this Task task)
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

    public static async Task SuppressWithMessage(this Task task)
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