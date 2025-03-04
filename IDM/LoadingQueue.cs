using KryptonM.IO;

namespace KryptonM.IDM;

public class LoadingQueue
{

    public int AsyncCount;
    public Action BeginTask = () => { };

    protected List<LoadingQueue> Children = new List<LoadingQueue>();
    protected bool DoneBasically;
    public Action EndTask = () => { };

    //filebase is used to generate relative resource file.
    //for example, a C:/s1/s2/s3.png is retargeted to s2/s3.png under a file base of C:/s1.
    public FileHandle Filebase = FileSystem.GetAbsolute("");

    public string Namespace;

    protected Queue<Action> PreLoadTasks = new Queue<Action>();
    public Dictionary<string, UploadQueueProcessor> Processors = new Dictionary<string, UploadQueueProcessor>();

    public float Progress;
    public float Run;
    protected Queue<Action> Tasks = new Queue<Action>();
    public float Total;

    public LoadingQueue(string namespc)
    {
        Namespace = namespc;
    }

    public virtual bool Done => DoneBasically && AsyncCount <= 0;

    public void SubLoader(LoadingQueue loader)
    {
        if(loader == null) return;

        Children.Add(loader);

        Total += loader.Total;

        FlushProgress();
    }

    private void Enqueue0(Action task, bool preLoad)
    {
        if(!preLoad)
        {
            Tasks.Enqueue(task);
            ++Total;
            DoneBasically = false;
        }
        else
        {
            PreLoadTasks.Enqueue(task);
        }
    }

    public void DoPreLoad()
    {
        while(PreLoadTasks.Count != 0) PreLoadTasks.Dequeue().Invoke();

        foreach(LoadingQueue c in Children) c.DoPreLoad();
    }

    public virtual void Next()
    {
        if(Tasks.Count == 0)
        {
            if(Children.Count == 0)
            {
                DoneBasically = true;
                Progress = 1;
                return;
            }

            LoadingQueue loader1 = Children[0];

            loader1.Next();
            ++Run;
            Progress = Run / Total;

            if(loader1.Done) Children.RemoveAt(0);
        }
        else
        {
            Tasks.Dequeue().Invoke();
            ++Run;
            Progress = Run / Total;
        }
    }

    public void FlushProgress()
    {
        Total = Tasks.Count;
        Run = 0;
        Progress = 0;
        DoneBasically = false;
    }

    public void Enqueue(Action task, bool preLoad)
    {
        Enqueue0(task, preLoad);
    }

    public void Enqueue(Action task)
    {
        Enqueue0(task, false);
    }

    public void Load(FileHandle file, bool preload = false)
    {
        string resName = file.Path.Replace(Filebase.Path + "/", "");
        AddLoadTask(resName, file, preload);
    }

    public void Scan(bool preload)
    {
        Scan(Filebase, preload);
    }

    public void Scan(FileHandle startPos, bool preload)
    {
        if(!startPos.Exists)
            return;
        FileHandle[] files = startPos.Directories;

        foreach(FileHandle file in files) Scan(file, preload);

        files = startPos.Files;

        foreach(FileHandle file in files) Load(file, preload);
    }

    private void AddLoadTask(string resource, FileHandle file, bool preLoad)
    {
        Enqueue(() =>
        {
            foreach(KeyValuePair<string, UploadQueueProcessor> kv in Processors)
                if(resource.StartsWith(kv.Key))
                    kv.Value(new ID(Namespace, resource), file);
        }, preLoad);
    }

}

public delegate void UploadQueueProcessor(ID resource, FileHandle file);