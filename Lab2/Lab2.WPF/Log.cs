using System.Diagnostics;
using System.IO;

namespace Lab2.WPF;
public static class Log {
    private static string LogFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log.txt");

    public static void Write(string logText) => WriteData(logText);
    public static void Write(string[] logArray) => Array.ForEach(logArray, line => WriteData(line));

    private static void WriteData(string logText) {
        if (!File.Exists(LogFile))
            File.Create(LogFile);

        using StreamWriter writer = new(LogFile);
        writer.WriteLine(logText);
        writer.Close();
    }

    public static void Show() {
        try {
            Process process = new Process();
            process.StartInfo.FileName = "notepad";
            process.StartInfo.Arguments = LogFile;
            process.StartInfo.UseShellExecute = false;
            process.Start();
            //Process.Start("notepad", LogFile);
        } catch (Exception ex) {
            Debug.WriteLine($"Couldn't open \"{LogFile}\": {ex.Message}\n{ex.StackTrace}");
        }
    }

    public static void Delete() => File.Delete(LogFile);
}
