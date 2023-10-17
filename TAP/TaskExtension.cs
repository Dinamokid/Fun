using System.Runtime.CompilerServices;
namespace Fun;

public static class TaskExtension
{
    public static TaskAwaiter GetAwaiter(this Func<Task>[] lambdas)
    {
        // var tasks = lambdas.Select(l => Task.Run(async delegate
        // {
        //     await l();
        // }));
        
        var tasks = lambdas.Select(l => Task.Run(async delegate
        {
            await l().ConfigureAwait(false);
        })).ToArray();
        
        return  Task.WhenAll(tasks).GetAwaiter();
    }
    
    public static TaskAwaiter GetAwaiter(this (Func<Task>, Func<Task>) paramsTuple)
    {
        var lambdas = new[]
        {
            paramsTuple.Item1, paramsTuple.Item2
        };
        
        var tasks = lambdas.Select(l => Task.Run(async delegate
        {
            await l().ConfigureAwait(false);
        })).ToArray();
        
        return  Task.WhenAll(tasks).GetAwaiter();
    }
}