using System.Diagnostics;
using System.IO;

namespace Lab6.Core.Output;
public static class Log {
    private static LinkedList<LogItem> _data = null!;
    private static string _last = string.Empty;

    public static void Initiate() => _data = [];

    public static void WriteLine() => Log.WriteLine("\n");

    public static void WriteLine(object? value, bool important = false) {
        if (value is null) {
            Log.WriteLine();
        } else {
            Log.WriteLine(value.ToString(), important);
        }
    }

    public static void WriteLine(string? value, bool important = false) {
        var item = new LogItem(value, important);
        _data.AddLast(item);
        Console.WriteLine(item);
    }

    public static void WriteLine(Exception exception) {
        Log.WriteLine(exception.Message);
        Debug.WriteLine(exception.StackTrace);
    }

    public static void Save() {
        _last = Path.Combine("logs", $"Log{DateTime.Now.ToShortDateString().Replace('.', '-')}_{DateTime.Now.ToLongTimeString().Replace(':', '-')}.txt");
        using (File.Create(_last)) { };

        if (File.Exists(_last)) {
            using StreamWriter writer = new(_last);
            writer.WriteLine(Log.GetLog(true));
            writer.Close();
        }
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

    public static string GetLog(bool all) => string.Join('\n', all ? _data : _data.Where(item => item.Important));
}
