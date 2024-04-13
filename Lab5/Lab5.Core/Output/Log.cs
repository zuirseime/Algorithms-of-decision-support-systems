using System.Diagnostics;
using System.IO;

namespace Lab5.Core.Output;
public static class Log {
    private static LinkedList<string> _data = null!;
    private static string _last = string.Empty;

    public static void Initiate() => _data = [];

    public static void WriteLine(string line = "") {
        _data.AddLast(line);
        Console.WriteLine(line);
    }

    public static void WriteLine(Exception exception) {
        Log.WriteLine(exception.Message);
        Debug.WriteLine(exception.StackTrace);
    }

    public static void Save() {
        _last = Path.Combine("logs", $"Log {DateTime.UtcNow}.txt");

        using StreamWriter writer = new(_last);
        writer.WriteLine(Log.GetLog());
        writer.Close();
    }

    public static void Show() {
        try {
            Process process = new();
            process.StartInfo.FileName = "notepad";
            process.StartInfo.Arguments = _last;
            process.Start();
        } catch (Exception ex) {
            Debug.WriteLine($"Couldn't open \"{_last}\": {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static void Clear() => _data?.Clear();

    public static string GetLog() => string.Join('\n', _data);
}
