using System.Reflection;

namespace KryptonM.IO;

public class FileSystem
{

    private static string CurPath;

    public static void AsApplicationSource()
    {
        CurPath = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName.Replace('\\', '/') + "/run";
    }

    public static FileHandle GetAbsolute(string path)
    {
        return new FileHandleImpl(path);
    }

    public static FileHandle GetLocal(string path)
    {
        return new FileHandleImpl(CurPath + "/" + path);
    }

}