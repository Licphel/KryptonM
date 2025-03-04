namespace KryptonM.IO;

public class FileHandleImpl : FileHandle
{

    public FileHandleImpl(string file)
    {
        Path = file.Replace('\\', '/');

        if(Path.EndsWith('/')) Path = Path.Substring(0, Path.Length - 1);

        IsFile = File.Exists(file);
        IsDirectory = Directory.Exists(file);
    }

    public void Delete()
    {
        if(IsFile)
            File.Delete(Path);
        else if(IsDirectory) Directory.Delete(Path, true);
    }

    public FileHandle[] Files
    {
        get
        {
            List<FileHandle> files = new List<FileHandle>();

            foreach(FileInfo info in new DirectoryInfo(Path).GetFiles()) files.Add(new FileHandleImpl(info.FullName));

            return files.ToArray();
        }
    }

    public FileHandle Goto(string name)
    {
        string call;

        if(Path.EndsWith("/"))
            call = Path + name;
        else
            call = Path + "/" + name;

        return new FileHandleImpl(call);
    }

    public bool Exists => IsFile || IsDirectory;

    public FileHandle Exit()
    {
        int idx = Path.LastIndexOf('/');
        string call = Path.Substring(0, idx);
        return new FileHandleImpl(call);
    }

    public FileHandle[] Directories
    {
        get
        {
            List<FileHandle> files = new List<FileHandle>();

            foreach(DirectoryInfo info in new DirectoryInfo(Path).GetDirectories())
                files.Add(new FileHandleImpl(info.FullName));

            return files.ToArray();
        }
    }

    public string Format
    {
        get
        {
            int idx = Path.LastIndexOf('.');
            return Path.Substring(idx + 1);
        }
    }

    public bool IsFile { get; }

    public string Path { get; }

    public bool IsDirectory { get; }

    public void Mkdirs()
    {
        DirectoryInfo info = new DirectoryInfo(Path);
        if(!info.Exists) info.Create();
    }

    public void Mkfile()
    {
        Exit().Mkdirs();
        FileInfo info = new FileInfo(Path);
        if(!info.Exists) info.Create().Close();
    }

    public string Name
    {
        get
        {
            if(IsDirectory)
            {
                int idx1 = Path.LastIndexOf('/') + 1;
                return Path.Substring(idx1);
            }

            if(IsFile)
            {
                int idx = Path.LastIndexOf('.');
                int idx1 = Path.LastIndexOf('/') + 1;
                return Path.Substring(idx1, idx - idx1);
            }

            throw new Exception($"File error: cannot find name of {Path}.");
        }
    }

}