using System.Diagnostics;
using System.IO;

namespace CalculatingWork.Core;
public static class Log {
    private struct Item(string? data, bool important) {
        public string? Data { get; set; } = data;
        public bool Important { get; set; } = important;

        public override readonly string ToString() => $"{this.Data}";
    }

    private static LinkedList<Item> _data = null!;
    private static string _last = string.Empty;

    public static void Initialize() => _data = [];

    public static void WriteLine() => Log.WriteLine("\n");

    public static void WriteLine(object? value, bool important = false) {
        if (value is null)
            Log.WriteLine();
        else {
            var strValue = value.ToString();
            var item = new Item(strValue, important);
            _data.AddLast(item);
            Console.WriteLine(strValue);
        }
    }

    public static void WriteLine(Exception ex) {
        Log.WriteLine(ex.Message);
        Debug.WriteLine(ex.StackTrace);
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
            Log.WriteLine($"Couldn't open \"{_last}\": {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static void Clear() {
        _data?.Clear();
    }
    public static string GetLog(bool all) => string.Join('\n', all ? _data : _data.Where(item => item.Important));
}
