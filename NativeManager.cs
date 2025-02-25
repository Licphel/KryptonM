namespace KryptonM;

public class NativeManager
{

    public static NativeManager I0 = new NativeManager();

    private readonly List<Action> ToRelease = new List<Action>();

    public void Remind(Action release)
    {
        ToRelease.Add(release);
    }

    public void Free()
    {
        ToRelease.ForEach(r => { r.Invoke(); });
        ToRelease.Clear();
    }

}