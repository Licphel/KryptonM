namespace KryptonM;

public class Coroutine
{

    private static readonly HashSet<Task> CurrentTasks = new HashSet<Task>();
    private Task _SysTask;

    public bool IsCompleted;
    public bool IsStarted;

    public Action TaskR;
    private Timer timer;
    private TimeSpan TimeUsed;

    public Coroutine(Action runnable)
    {
        TaskR = runnable;
    }

    public void Start()
    {
        CurrentTasks.Add(_SysTask = Task.Factory.StartNew(() =>
        {
            DateTime d1 = DateTime.Now;
            IsStarted = true;
            TaskR();
            IsCompleted = true;
            Dispose();
            TimeUsed = DateTime.Now - d1;
        }));
    }

    public void Dispose()
    {
        timer?.Dispose();
        CurrentTasks.Remove(_SysTask);
    }

    public static void Wait()
    {
        Logger.Info("Executing remaining async tasks...", true);
        Task.WaitAll(CurrentTasks.ToArray());
        Logger.Fix("Done!");
    }

}