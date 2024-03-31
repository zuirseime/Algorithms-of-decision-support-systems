using System.Diagnostics;
using System.IO;

namespace Lab4.Core.Output;

public sealed class Log {
    private string _path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

    private static Log instance = null!;
    public static Log Instance => instance ??= new Log();

    private List<string> _data = null!;
    private Log() => _data = [];

    public void WriteLine(string line) {
        _data.Add(line);
        Console.WriteLine(line);
        Debug.WriteLine(line);
    }

    public void Save() {
        if (!File.Exists(_path))
            File.Create(_path);

        using StreamWriter writer = new(_path);
        writer.WriteLine(string.Join("\n", _data));
        writer.Close();
    }

    public void Show() {
        try {
            Process process = new();
            process.StartInfo.FileName = "notepad";
            process.StartInfo.Arguments = _path;
            process.StartInfo.UseShellExecute = false;
            process.Start();
        } catch (Exception ex) {
            Debug.WriteLine($"Couldn't open \"{_path}\": {ex.Message}\n{ex.StackTrace}");
        }
    }

    public void Clear() => _data?.Clear();

    public void Delete() => File.Delete(_path);

    public override string ToString() {
        string result = string.Empty;
        _data.ForEach(line => result += $"{line}\n");

        return result;
    }
}