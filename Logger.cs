using System.Diagnostics;
using System.Reflection;
using System.Text;
using KryptonM.IO;

namespace KryptonM;

public class Logger
{

    private const string
        DEBUG = "DEBUG",
        INFO = "INFO",
        WARN = "WARN",
        FATAL = "FATAL",
        STACKTRACE = "STACKTRACE";

    public static int Lines;

    private static StreamWriter outw;
    private static string levelPrev;
    private static readonly List<string> LineCache = new List<string>();
    private static readonly Queue<int> TailStack = new Queue<int>();

    public static void StartStreamWriting(FileHandle file)
    {
        if(!file.Exists)
        {
            file.Mkfile();
        }
        else
        {
            file.Delete();
            file.Mkfile(); //Reflush
        }

        outw = new StreamWriter(file.Path, true, Encoding.UTF8);
    }

    public static void TryEndStreamWriting()
    {
        foreach(var s in LineCache)
            outw.WriteLine(s);
        outw.Close();
        outw = null;
    }

    private static void Print(string level, string info)
    {
        Lines++;
        var className = "-";

        StackTrace stacks = new StackTrace(1);
        StackFrame frame = stacks.GetFrame(1);

        if(frame != null)
        {
            MethodBase method = frame.GetMethod();

            if(method != null && method.ReflectedType != null)
            {
                className = method.ReflectedType.Name;

                if(className.StartsWith('<')) className = "-";
            }
        }

        var threadName = Thread.CurrentThread.Name;

        if(string.IsNullOrWhiteSpace(threadName)) threadName = "-";

        var time = DateTime.Now.ToString("u");

        var outs = $"[{level}] [{threadName}] [{className}] [{time}] {info}";

        switch(level)
        {
            case DEBUG:
                Console.ForegroundColor = ConsoleColor.DarkGray;
                break;
            case INFO:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case WARN:
                Console.ForegroundColor = ConsoleColor.DarkRed;
                break;
            case FATAL:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
            case STACKTRACE:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
        }

        if(level != DEBUG) Console.WriteLine(outs);

        LineCache.Add(outs);

        levelPrev = level;
    }

    public static void Fix(string info)
    {
        var line = TailStack.Dequeue();
        var s = LineCache[line];

        if(levelPrev != DEBUG)
        {
            var pos1 = Console.CursorLeft;
            var pos2 = Console.CursorTop;
            try
            {
                Console.SetCursorPosition(s.Length, line);
                Console.Write(" ");
                Console.Write(info);
                Console.SetCursorPosition(pos1, pos2);
            }
            catch
            {
            }
        }

        LineCache[line] = s + " " + info;
    }

    public static void Debug(string info, bool tail = false)
    {
        if(tail)
            TailStack.Enqueue(Lines);
        Print(DEBUG, info);
    }

    public static void Info(string info, bool tail = false)
    {
        if(tail)
            TailStack.Enqueue(Lines);
        Print(INFO, info);
    }

    public static void Warn(string info, bool tail = false)
    {
        if(tail)
            TailStack.Enqueue(Lines);
        Print(WARN, info);
    }

    public static void Fatal(string info, bool tail = false)
    {
        if(tail)
            TailStack.Enqueue(Lines);
        Print(FATAL, info);
    }

    public static void Warn(Exception exc)
    {
        Print(WARN, exc.Message);
        Print(STACKTRACE, exc.StackTrace);
    }

    public static void Fatal(Exception exc)
    {
        Print(FATAL, exc.Message);
        Print(STACKTRACE, exc.StackTrace);
    }

    public static void NextLine()
    {
        outw.WriteLine("\n");
    }

}